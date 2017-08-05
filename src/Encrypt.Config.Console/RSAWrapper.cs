using System;
using System.Linq;
using System.Security.Cryptography;

namespace Encrypt.Config.Console {
    public class RSAWrapper : IDisposable
    {
        private readonly RSACryptoServiceProvider _rsaCryptoServiceProvider;

        public RSAWrapper(RSACryptoServiceProvider rsaCryptoServiceProvider)
        {
            _rsaCryptoServiceProvider = rsaCryptoServiceProvider;
        }

        public RsaExport Export(bool includePrivate)
        {
            return new RsaExport()
            {
                Key = _rsaCryptoServiceProvider.ToXmlString(includePrivate),

                RsaParameters = _rsaCryptoServiceProvider.ExportParameters(includePrivate)
            };
        }

        public string ContainerName()
        {
            return _rsaCryptoServiceProvider.CspKeyContainerInfo.KeyContainerName;
        }

        public byte[] Encrypt(byte[] data, byte[] salt)
        {
            return _rsaCryptoServiceProvider.Encrypt(salt.Concat(data).ToArray(), RSAEncryptionPadding.Pkcs1);
        }

        public byte[] Decrypt(byte[] data, byte[] salt)
        {
            return _rsaCryptoServiceProvider.Decrypt(data, RSAEncryptionPadding.Pkcs1).Skip(salt.Length).ToArray();
        }

        public void Dispose()
        {
            _rsaCryptoServiceProvider.Dispose();
        }

        public string UniqueContainerName()
        {
            return _rsaCryptoServiceProvider.CspKeyContainerInfo.UniqueKeyContainerName;
        }
    }
}