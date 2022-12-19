using System.Security.Cryptography;
using System.Text;

namespace IdentityJwtPoc.Infra.Data.CrossCutting.Cryptography
{
    public class Cryptography
    {
        private const string KEY = "D4FE*A52@3A64$57";

        public static string EncryptString(string plainInput)
        {
            byte[] iv = new byte[16];
            byte[] array;
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(KEY);
            aes.IV = iv;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
            using (StreamWriter streamWriter = new((Stream)cryptoStream))
                streamWriter.Write(plainInput);
            array = memoryStream.ToArray();
            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(KEY);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using MemoryStream memoryStream = new(buffer);
            using CryptoStream cryptoStream = new((Stream)memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new((Stream)cryptoStream);
            return streamReader.ReadToEnd();
        }
    }
}
