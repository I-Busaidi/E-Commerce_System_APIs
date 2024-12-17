namespace E_Commerce_System.Services
{
    public interface IJwtService
    {
        (int userId, string userName, string role) DecodeToken(string token);
        string GenerateJwtToken(string userId, string userName, string role);
    }
}