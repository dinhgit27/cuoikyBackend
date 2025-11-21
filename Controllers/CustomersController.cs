using Microsoft.AspNetCore.Mvc;
using btapbackend.Data;
using btapbackend.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
[Route("api/customers")]
[ApiController]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _context;

    public CustomersController(AppDbContext context) => _context = context;

    // GET: api/customers → Chỉ Admin
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers()
    {
        return await _context.Customers
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                Role = c.Role
            })
            .ToListAsync();
    }

    // GET: api/customers/me → Ai cũng được xem thông tin mình
    [HttpGet("me")]
    public async Task<ActionResult<CustomerDto>> GetMyInfo()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var customer = await _context.Customers.FindAsync(userId);

        if (customer == null) return NotFound();

        return Ok(new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            Role = customer.Role
        });
    }

    // DELETE: api/customers/5 → Chỉ Admin
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Role { get; set; }
}