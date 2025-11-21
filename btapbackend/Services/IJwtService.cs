using btapbackend.Entities;

namespace btapbackend.Services
{
    public interface IJwtService
    {
        string GenerateToken(Customer customer);
    }
}