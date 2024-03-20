using CatalogService.Data;
using CatalogService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
  private readonly CatalogContext _context;

  public CatalogController(CatalogContext context)
  {
    _context = context;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
  {
    return await _context.Products.ToListAsync();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Product>> GetProductById(int id)
  {
    var product = await _context.Products.FindAsync(id);
    return product == null
      ? NotFound()
      : product;
  }

  [HttpPost]
  public async Task<ActionResult<Product>> CreateProduct(Product product)
  {
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        return StatusCode(500, "An error occured while creating the product.");
    }
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
  {
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
      var productToEdit = await _context.Products.FindAsync(id);
      if (productToEdit == null)
      {
        return NotFound();
      }
      if (product.Name.Length < 3 || product.Price < 0 || product.Inventory < 0)
      {
        return BadRequest();
      }
      productToEdit.Name = product.Name;
      productToEdit.Price = product.Price;
      productToEdit.Inventory = product.Inventory;
      await _context.SaveChangesAsync();
      await transaction.CommitAsync();
      return Ok(product);
    }
    catch (Exception)
    {
      await transaction.RollbackAsync();
      return StatusCode(500, "An error occured while updating the product.");
    }
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Product>> UpdateInventory(int id, int inventoryChange)
  {
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
      var product = await _context.Products.FindAsync(id);
      if (product == null)
      {
        return NotFound();
      }
      if (product.Inventory + inventoryChange < 0)
      {
        return BadRequest("Inventory cannot be negative");
      }
      product.Inventory += inventoryChange;
      await _context.SaveChangesAsync();
      await transaction.CommitAsync();
      return Ok(product);
    }
    catch (Exception)
    {
      await transaction.RollbackAsync();
      return StatusCode(500, "An error occured while updating the product.");
    }
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Product>> DeleteProduct(int id)
  {
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
      var product = await _context.Products.FindAsync(id);
      if (product == null)
      {
        return NotFound();
      }
      _context.Products.Remove(product);
      await _context.SaveChangesAsync();
      await transaction.CommitAsync();
      return NoContent();
    }
    catch (Exception)
    {
      await transaction.RollbackAsync();
      return StatusCode(500, "An error occured while deleting the product.");
    }
  }
}