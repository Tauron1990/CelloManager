namespace Tauron.Application.CelloManager.Logic
{
    public interface IPasswordHasher
    {
        string GetPassword(string hash);
        string HashPassword(string password);
    }
}