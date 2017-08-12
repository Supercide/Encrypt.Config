using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.Random;
using Newtonsoft.Json.Linq;
using Encrypt.Config.Encryption.Symmetric;

namespace Encrypt.Config.Json
{
    public class FileEncrypter
    {
        private readonly IHybridEncryption _hybridEncryption;
        private readonly IGenerateRandomBytes _randomBytesGenerator;

        public FileEncrypter(IHybridEncryption hybridEncryption, IGenerateRandomBytes randomBytesGenerator)
        {
            _hybridEncryption = hybridEncryption;

            _randomBytesGenerator = randomBytesGenerator;
        }

        public (EncryptionKey key, byte[] data) Encrypt(string filePath)
        {
            var file = File.ReadAllBytes(filePath);

            var sessionKey = _randomBytesGenerator.Generate(32);

            var iv = _randomBytesGenerator.Generate(16);

            return _hybridEncryption.EncryptData(sessionKey, file, iv);
        }

        public byte[] Decrypt(string keyPath, byte[] encryptedData)
        {
            var keyBlob = File.ReadAllBytes(keyPath);

            var encryptionKey = EncryptionKey.FromBlob(keyBlob);

            return _hybridEncryption.DecryptData(encryptionKey, encryptedData);
        }
    }
}
