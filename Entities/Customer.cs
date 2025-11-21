namespace btapbackend.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = "User"; // Admin hoặc User
    }
}