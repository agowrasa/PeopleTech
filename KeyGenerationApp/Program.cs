using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        string secureKey = GenerateSecureKey();
        Console.WriteLine("Secure Key: " + secureKey);
    }

    static string GenerateSecureKey()
    {
        int keyLength = 64; // 64 bytes (512 bits)
        byte[] randomBytes = new byte[keyLength];

        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(randomBytes);
        }

        string secureKey = Convert.ToBase64String(randomBytes);
        return secureKey;
    }
}
