using Microsoft.AspNetCore.Mvc;
using PurchasingService.Models;
using PurchasingService.Services;

namespace PurchasingService.Controllers;

[ApiController]
[Route("[controller]")]
public class InquiryController : ControllerBase
{
    private readonly IInquiryService _inquiryService;

    public InquiryController(IInquiryService inquiryService)
    {
        _inquiryService = inquiryService;
    }

    [HttpPost("requestOffer")]
    public async Task<ActionResult<OfferResponse>> GetOffer([FromBody] OfferRequest request)
    {
        if (request is null)
        {
            return BadRequest("Request payload is required.");
        }

        if (request.OfferDetails is null || request.OfferDetails.Count == 0)
        {
            return BadRequest("At least one product must be provided.");
        }

        try
        {
            var response = await _inquiryService.RequestOfferAsync(request);
            
            // Check if email was provided and offer was sent
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                return Ok("Offer has been sent successfully.");
            }

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
