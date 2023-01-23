using System.Web.Helpers;

namespace casa_benjamin.Helpers
{
    public static class CryptographyHelper
    {
        public static string HashPassword(string password)
        {
            return Crypto.HashPassword(password);
        }

        public static bool VerifyPassword(string password,string hash)
        {
            bool success = Crypto.VerifyHashedPassword(hash, password);
            return success;
        }
    }
}