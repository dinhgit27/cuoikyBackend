using Microsoft.AspNetCore.Mvc;
using btapbackend.Data;
using btapbackend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
[Route("api/products")]
[ApiController]
[Authorize] // tất cả đều cần đăng nhập
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context) => _context = context;

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _context.Products
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Stock = p.Stock
            })
            .ToListAsync();
        return Ok(products);
    }

    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        return Ok(new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            Stock = product.Stock
        });
    }

    // POST: api/products → Chỉ Admin
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> CreateProduct(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            Stock = dto.Stock
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    // PUT: api/products/5 → Chỉ Admin
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.Description = dto.Description;
        product.Stock = dto.Stock;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/products/5 → Chỉ Admin
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

// DTOs
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public int Stock { get; set; }
}

public class CreateProductDto
{
    [Required] public string Name { get; set; }
    [Range(0, double.MaxValue)] public decimal Price { get; set; }
    public string Description { get; set; }
    [Range(0, int.MaxValue)] public int Stock { get; set; }
}

public class UpdateProductDto : CreateProductDto { }