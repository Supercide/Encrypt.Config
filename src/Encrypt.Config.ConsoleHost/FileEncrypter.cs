﻿using System.IO;
using Encrypt.Config.Encryption;
using Encrypt.Config.Encryption.Hybrid;
using Encrypt.Config.Encryption.NumberGenerators;

namespace Encrypt.Config.ConsoleHost
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

        public (EncryptionSettings key, byte[] data) Encrypt(string filePath)
        {
            var file = File.ReadAllBytes(filePath);

            var sessionKey = _randomBytesGenerator.Generate(32);

            var iv = _randomBytesGenerator.Generate(16);

            return _hybridEncryption.EncryptData(sessionKey, file, iv);
        }
    }
}
