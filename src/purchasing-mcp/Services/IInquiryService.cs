using PurchasingService.Models;

namespace PurchasingService.Services;

public interface IInquiryService
{
    Task<Offer> RequestOfferAsync(OfferRequest request);
}
