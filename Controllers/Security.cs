using System;
using System.Security.Cryptography;
using System.Text;

namespace CarMileage.Controllers
{
    public static class Security
    {
        private const int numberOfIterations = 100;
        private static string generatePassword(string passwordToHash, int numberOfIterations)
        {
            var salt = new byte[24];
            new RNGCryptoServiceProvider().GetBytes(salt);
            var pbkdf2 = new Rfc2898DeriveBytes(passwordToHash, salt, numberOfIterations);
            byte[] hash = pbkdf2.GetBytes(24);

            return Convert.ToBase64String(hash) + "|" + Convert.ToBase64String(salt); ;
        }
        public static bool checkPassword(string passwordUnencrypted, string passwordWithHash, int numberOfIterations = numberOfIterations)
        {
            if (!passwordWithHash.Contains('|'))
            {
                return false;
            }
            var origHashedParts = passwordWithHash.Split('|');
            var hashedPassword = origHashedParts[0];
            var sault = Convert.FromBase64String(origHashedParts[1]);
            var pbkdf2 = new Rfc2898DeriveBytes(passwordUnencrypted, sault, numberOfIterations);
            byte[] testHash = pbkdf2.GetBytes(24);
            if (Convert.ToBase64String(testHash) == hashedPassword)
                return true;

            return false;

        }


    }
}