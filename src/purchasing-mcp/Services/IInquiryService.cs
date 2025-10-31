using PurchasingService.Models;

namespace PurchasingService.Services;

public interface IInquiryService
{
    Task<Offer> RequestOfferAsync(OfferRequest request);
    Task<Offer?> GetOfferByIdAsync(Guid offerId);
    Task<List<Offer>> GetOffersByRequestIdAsync(string requestId);
}
