using System.Linq;
using System.Security.Cryptography;

namespace Encrypt.Config.RSA {
    public static class RSACryptoServiceProviderExtensions
    {
        public static byte[] Decrypt(this RSACryptoServiceProvider provider, byte[] data, RSAEncryptionPadding padding, byte[] salt)
        {
            return provider.Decrypt(data, RSAEncryptionPadding.Pkcs1).Skip(salt.Length).ToArray();
        }

        public static byte[] Encrypt(this RSACryptoServiceProvider provider, byte[] data, RSAEncryptionPadding padding, byte[] salt)
        {
            return provider.Encrypt(salt.Concat(data).ToArray(), RSAEncryptionPadding.Pkcs1);
        }
    }
}