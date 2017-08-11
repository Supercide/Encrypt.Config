using System.Security.Cryptography;
using Encrypt.Config.Encryption.Asymmetric;

namespace Encrypt.Config.Encryption.RSA {
    public class RSAEncryption : IAsymmetricKeyEncryption
    {
        private readonly string _containerName;
        private readonly string _username;

        public RSAEncryption()
        {
            
        }

        public RSAEncryption(string containerName)
        {
            _containerName = containerName;
        }

        public RSAEncryption(string containerName, string username)
        {
            _containerName = containerName;
            _username = username;
        }

        public string ExportKey(bool includePrivate)
        {
            using (RSACryptoServiceProvider rsaCryptoServiceProvider = RSAContainerFactory.Create(_containerName, _username))
            {
                return rsaCryptoServiceProvider.ToXmlString(includePrivate);
            }
        }

        public byte[] EncryptData(byte[] data, string publicKey)
        {
            using (RSACryptoServiceProvider rsaCryptoServiceProvider = RSAContainerFactory.CreateFromPublicKey(publicKey))
            {
                rsaCryptoServiceProvider.FromXmlString(publicKey);

                return rsaCryptoServiceProvider.Encrypt(data, RSAEncryptionPadding.Pkcs1);
            }
        }

        public byte[] DecryptData(byte[] data)
        {
            using(RSACryptoServiceProvider rsaCryptoServiceProvider = RSAContainerFactory.Create(_containerName, _username))
            {
                return rsaCryptoServiceProvider.Decrypt(data, RSAEncryptionPadding.Pkcs1);
            }
        }
    }
}