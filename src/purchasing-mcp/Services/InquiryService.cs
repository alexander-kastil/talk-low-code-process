using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using PurchasingService.Data;
using PurchasingService.Graph;
using PurchasingService.Models;

namespace PurchasingService.Services;

public class InquiryService : IInquiryService
{
    private readonly IOfferRandomizer _offerRandomizer;
    private readonly GraphHelper _graphHelper;
    private readonly IChatClient _chatClient;
    private readonly PurchasingDbContext _dbContext;

    public InquiryService(IOfferRandomizer offerRandomizer, GraphHelper graphHelper, IChatClient chatClient, PurchasingDbContext dbContext)
    {
        _offerRandomizer = offerRandomizer ?? throw new ArgumentNullException(nameof(offerRandomizer));
        _graphHelper = graphHelper ?? throw new ArgumentNullException(nameof(graphHelper));
        _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Offer> RequestOfferAsync(OfferRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.RequestDetails is null || request.RequestDetails.Count == 0)
        {
            throw new ArgumentException("At least one product must be provided.", nameof(request));
        }

        var supplier = SupplierStore.GetSupplierById(request.SupplierId);

        if (supplier is null)
        {
            throw new InvalidOperationException($"Supplier with id {request.SupplierId} was not found.");
        }

        var offerLines = new List<OfferDetails>(request.RequestDetails.Count);

        foreach (var productRequest in request.RequestDetails)
        {
            if (productRequest is null)
            {
                throw new ArgumentException("Product entries cannot be null.", nameof(request));
            }

            var isOffered = supplier.AvailableProducts.Contains(productRequest.Product, StringComparer.OrdinalIgnoreCase);

            if (isOffered)
            {
                offerLines.Add(_offerRandomizer.GenerateOffer(productRequest.Product, productRequest.RequestedQuantity));
            }
            else
            {
                offerLines.Add(new OfferDetails
                {
                    ProductName = productRequest.Product,
                    Price = 0,
                    RequestedQuantity = productRequest.RequestedQuantity,
                    Quantity = 0,
                    DeliveryDurationDays = 0
                });
            }
        }

        var response = new Offer
        {
            SupplierId = supplier.SupplierId,
            TransportationCost = _offerRandomizer.TransportationCost,
            Timestamp = DateTimeOffset.UtcNow,
            OfferDetails = offerLines,
            Email = request.Email?.Trim()
        };

        // Save the offer to the database
        _dbContext.Offers.Add(response);
        await _dbContext.SaveChangesAsync();

        await ResponseHandler.TrySendOfferAsync(_graphHelper, _chatClient, response).ConfigureAwait(false);

        return response;
    }
}
