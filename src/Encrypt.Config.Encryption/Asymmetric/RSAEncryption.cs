using System.Security.Cryptography;

namespace Encrypt.Config.Encryption.Asymmetric {
    public class RSAEncryption : IAsymmetricKeyEncryption
    {
        private readonly string _containerName;
        private readonly string _username;
        private string _publicKey;
        

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

            RSAContainerFactory.Create(containerName, username)
                               .Dispose();
        }

        public static RSAEncryption FromNewContainer(string containerName, string username)
        {
            return new RSAEncryption(containerName, username);
        }

        public static RSAEncryption FromExistingContainer(string containerName)
        {
            return new RSAEncryption(containerName);
        }

        public static RSAEncryption FromPublicKey(string publicKey)
        {
            return new RSAEncryption
            {
                _publicKey = publicKey
            };
        }

        public string ExportKey(bool includePrivate)
        {
            using (RSACryptoServiceProvider rsaCryptoServiceProvider = RSAContainerFactory.CreateFromContainer(_containerName))
            {
                return rsaCryptoServiceProvider.ToXmlString(includePrivate);
            }
        }

        public byte[] EncryptData(byte[] data)
        {
            if(_publicKey == null)
            {
                using (RSACryptoServiceProvider rsaCryptoServiceProvider = RSAContainerFactory.Create(_containerName, _username))
                {
                    return rsaCryptoServiceProvider.Encrypt(data, RSAEncryptionPadding.Pkcs1);
                }
            }

            using (RSACryptoServiceProvider rsaCryptoServiceProvider = RSAContainerFactory.CreateFromPublicKey(_publicKey))
            {
                return rsaCryptoServiceProvider.Encrypt(data, RSAEncryptionPadding.Pkcs1);
            }
        }

        public byte[] DecryptData(byte[] data)
        {
            using (RSACryptoServiceProvider rsaCryptoServiceProvider = RSAContainerFactory.CreateFromContainer(_containerName))
            {
                return rsaCryptoServiceProvider.Decrypt(data, RSAEncryptionPadding.Pkcs1);
            }
        }
    }
}