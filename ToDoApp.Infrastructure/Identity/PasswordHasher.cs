using ToDoApp.Application.Common.Interfaces;

namespace ToDoApp.Infrastructure.Identity;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password)));
    }

    public bool VerifyHashedPassword(string hashedPassword, string password)
    {
        return hashedPassword == HashPassword(password);
    }
}