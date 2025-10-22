using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PurchasingService.Data;

namespace PurchasingService.Controllers;

[ApiController]
[Route("[controller]")]
public class SuppliersController : ControllerBase
{
    [HttpGet("getSuppliers")]
    public ActionResult<IEnumerable<Supplier>> GetSuppliers()
    {
        return Ok(SupplierStore.GetSuppliers());
    }

    [HttpGet("getSupplierByID/{id:int}")]
    public ActionResult<Supplier> GetSupplierById(int id)
    {
        var supplier = SupplierStore.GetSupplierById(id);
        return supplier is null ? NotFound() : Ok(supplier);
    }

    [HttpGet("getSupplierByName/{name}")]
    public ActionResult<Supplier> GetSupplierByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Name must be provided.");
        }

        var supplier = SupplierStore.GetSupplierByName(name);

        return supplier is null ? NotFound() : Ok(supplier);
    }

    [HttpGet("getSupplierFor/{product}")]
    public ActionResult<IEnumerable<Supplier>> GetSupplierFor(string product)
    {
        if (string.IsNullOrWhiteSpace(product))
        {
            return BadRequest("Product name must be provided.");
        }

        var matches = SupplierStore.GetSuppliers()
            .Where(s => s.Products.Any(p => string.Equals(p, product, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        return matches.Count == 0 ? NotFound() : Ok(matches);
    }
}
