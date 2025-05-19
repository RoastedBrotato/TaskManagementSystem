using System.Security.Cryptography;
using System.Text;
using TaskManagementSystem.Core.Interfaces;

namespace TaskManagementSystem.Api.Services
{
    public class PasswordHashService : IPasswordHashService
    {
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool VerifyPassword(string hashedPassword, string password)
        {
            var newHashedPassword = HashPassword(password);
            return newHashedPassword == hashedPassword;
        }
    }
}