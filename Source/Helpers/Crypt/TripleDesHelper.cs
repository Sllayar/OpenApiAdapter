using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace RFI.Helpers.Crypt
{
    public static class TripleDESHelper
    {
        public static string Encrypt(string plainText, out string desParameters)
        {
            using(var tripleDesCryptoServiceProvider = new TripleDESCryptoServiceProvider())
            {
                desParameters = JsonSerializer.Serialize(new DesParameters { Key = Convert.ToBase64String(tripleDesCryptoServiceProvider.Key), Vector = Convert.ToBase64String(tripleDesCryptoServiceProvider.IV) });
                using(var encryptor = tripleDesCryptoServiceProvider.CreateEncryptor(tripleDesCryptoServiceProvider.Key, tripleDesCryptoServiceProvider.IV))
                using(var memoryStream = new MemoryStream())
                using(var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using(var streamWriter = new StreamWriter(cryptoStream)) streamWriter.Write(plainText);
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText, string desDecrypted)
        {
            var desParameters = JsonSerializer.Deserialize<DesParameters>(desDecrypted);
            if(desParameters.Key == null || desParameters.Vector == null) throw new Exception("des parameters error");

            using(var tripleDesCryptoServiceProvider = new TripleDESCryptoServiceProvider())
            {
                using(var decryptor = tripleDesCryptoServiceProvider.CreateDecryptor(Convert.FromBase64String(desParameters.Key), Convert.FromBase64String(desParameters.Vector)))
                using(var memoryStream = new MemoryStream(Convert.FromBase64String(cipherText)))
                using(var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using(var streamReader = new StreamReader(cryptoStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}