using Microsoft.AspNetCore.Mvc;
using btapbackend.Data;
using btapbackend.Services;
using btapbackend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthController(AppDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        if (_context.Customers.Any(c => c.Email == dto.Email))
            return BadRequest("Email đã tồn tại");

        var customer = new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "User" // mặc định là User
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Register), new { id = customer.Id }, "Đăng ký thành công");
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == dto.Email);

        if (customer == null || !BCrypt.Net.BCrypt.Verify(dto.Password, customer.PasswordHash))
            return Unauthorized("Sai email hoặc mật khẩu");

        var token = _jwtService.GenerateToken(customer);

        return Ok(new LoginResponseDto
        {
            Token = token,
            User = new UserInfoDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Role = customer.Role
            }
        });
    }
}

// DTOs
public class RegisterDto
{
    [Required] public string Name { get; set; }
    [Required, EmailAddress] public string Email { get; set; }
    [Required, MinLength(6)] public string Password { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

public class LoginDto
{
    [Required, EmailAddress] public string Email { get; set; }
    [Required] public string Password { get; set; }
}

public class LoginResponseDto
{
    public string Token { get; set; }
    public UserInfoDto User { get; set; }
}

public class UserInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}