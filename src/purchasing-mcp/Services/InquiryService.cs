using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using PurchasingService.Data;
using PurchasingService.Graph;
using PurchasingService.Models;

namespace PurchasingService.Services;

public class InquiryService : IInquiryService
{
    private readonly IOfferRandomizer _offerRandomizer;
    private readonly GraphHelper? _graphHelper;
    private readonly IChatClient? _chatClient;
    private readonly PurchasingDbContext _dbContext;

    public InquiryService(IOfferRandomizer offerRandomizer, GraphHelper? graphHelper, IChatClient? chatClient, PurchasingDbContext dbContext)
    {
        _offerRandomizer = offerRandomizer ?? throw new ArgumentNullException(nameof(offerRandomizer));
        _graphHelper = graphHelper;
        _chatClient = chatClient;
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

        var supplier = await _dbContext.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.SupplierId == request.SupplierId);

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

            var isOffered = supplier.Products.Any(p => string.Equals(p.Name, productRequest.Product, StringComparison.OrdinalIgnoreCase));

            if (isOffered)
            {
                offerLines.Add(await _offerRandomizer.GenerateOfferAsync(productRequest.Product, productRequest.RequestedQuantity));
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

        if (_graphHelper != null && _chatClient != null)
        {
            await EmailResponseHandler.TrySendOfferAsync(_graphHelper, _chatClient, response).ConfigureAwait(false);
        }

        return response;
    }

    public async Task<Offer?> GetOfferByIdAsync(Guid offerId)
    {
        return await _dbContext.Offers
            .Include(o => o.OfferDetails)
            .FirstOrDefaultAsync(o => o.OfferId == offerId);
    }
}
