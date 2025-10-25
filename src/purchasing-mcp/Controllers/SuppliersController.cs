using Microsoft.AspNetCore.Mvc;
using PurchasingService.Services;

namespace PurchasingService.Controllers;

[ApiController]
[Route("[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));
    }

    [HttpGet("getSuppliers")]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
    {
        var suppliers = await _supplierService.GetAllSuppliersAsync();
        return Ok(suppliers);
    }

    [HttpGet("getSupplierByID/{id:int}")]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetSupplierById(int id)
    {
        var suppliers = await _supplierService.GetSupplierByIdAsync(id);
        return suppliers.Count == 0 ? NotFound() : Ok(suppliers);
    }

    [HttpGet("getSupplierByName/{name}")]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetSupplierByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Name must be provided.");
        }

        var suppliers = await _supplierService.GetSupplierByNameAsync(name);

        return suppliers.Count == 0 ? NotFound() : Ok(suppliers);
    }

    [HttpGet("getSupplierFor/{product}")]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetSupplierFor(string product)
    {
        if (string.IsNullOrWhiteSpace(product))
        {
            return BadRequest("Product name must be provided.");
        }

        var matches = await _supplierService.GetSuppliersForProductAsync(product);

        return matches.Count == 0 ? NotFound() : Ok(matches);
    }
}
