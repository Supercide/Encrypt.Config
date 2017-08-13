using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using Encrypt.Config.Encryption;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.Hybrid;
using Encrypt.Config.Encryption.Symmetric;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests.Encryption.HybridEncryptTests {
    public class WhenImportingKey
    {
        [Test]
        public void GivenEncryptionKeyBlob_WhenImportingKey_ThenImportsIVCorrectly()
        {
            var currentUser = WindowsIdentity.GetCurrent()
                                             .Name;

            HybridEncryption hybridEncryption = new HybridEncryption(new RSAEncryption("somecontainer", currentUser), new AESEncryption());

            RandomNumberGenerator random = new RNGCryptoServiceProvider();

            var data = File.ReadAllBytes("appsettings.json");
            var sessionKey = new byte[32];
            var iv = new byte[16];

            random.GetBytes(sessionKey);
            random.GetBytes(iv);

            (EncryptionKey key, byte[] encryptedData) encryptedResult = hybridEncryption.EncryptData(sessionKey, data, iv);

            var key = encryptedResult.key;

            var keyBlob = key.ExportToBlob();

            var keyFromBlob = EncryptionKey.FromBlob(keyBlob);

            Assert.That(keyFromBlob.IV, Is.EqualTo(key.IV));
        }

        [Test]
        public void GivenEncryptionKeyBlob_WhenImportingKey_ThenImportsHMACHashCorrectly()
        {
            var currentUser = WindowsIdentity.GetCurrent()
                                             .Name;

            HybridEncryption hybridEncryption = new HybridEncryption(new RSAEncryption("somecontainer", currentUser), new AESEncryption());

            RandomNumberGenerator random = new RNGCryptoServiceProvider();

            var data =File.ReadAllBytes("appsettings.json");
            var sessionKey = new byte[32];
            var iv = new byte[16];

            random.GetBytes(sessionKey);
            random.GetBytes(iv);

            (EncryptionKey key, byte[] encryptedData) encryptedResult = hybridEncryption.EncryptData(sessionKey, data, iv);

            var key = encryptedResult.key;

            var keyBlob = key.ExportToBlob();

            var keyFromBlob = EncryptionKey.FromBlob(keyBlob);

            Assert.That(keyFromBlob.HMACHash, Is.EqualTo(key.HMACHash));
        }

        [Test]
        public void GivenEncryptionKeyBlob_WhenImportingKey_ThenImportsSessionKeyCorrectly()
        {
            var currentUser = WindowsIdentity.GetCurrent()
                                             .Name;

            HybridEncryption hybridEncryption = new HybridEncryption(new RSAEncryption("somecontainer", currentUser), new AESEncryption());

            RandomNumberGenerator random = new RNGCryptoServiceProvider();

            var data = File.ReadAllBytes("appsettings.json");
            var sessionKey = new byte[32];
            var iv = new byte[16];

            random.GetBytes(sessionKey);
            random.GetBytes(iv);

            (EncryptionKey key, byte[] encryptedData) encryptedResult = hybridEncryption.EncryptData(sessionKey, data, iv);

            var key = encryptedResult.key;

            var keyBlob = key.ExportToBlob();

            var keyFromBlob = EncryptionKey.FromBlob(keyBlob);

            Assert.That(keyFromBlob.SessionKey, Is.EqualTo(key.SessionKey));
        }

        [Test]
        public void GivenEncryptionKeyBlob_WhenImportingKey_ThenDecryptsSessionKeyCorrectly()
        {
            var currentUser = WindowsIdentity.GetCurrent()
                                             .Name;

            var asymmetricKeyEncryption = new RSAEncryption("somecontainer", currentUser);
            HybridEncryption hybridEncryption = new HybridEncryption(asymmetricKeyEncryption, new AESEncryption());

            RandomNumberGenerator random = new RNGCryptoServiceProvider();

            var data = File.ReadAllBytes("appsettings.json");
            var sessionKey = new byte[32];
            var iv = new byte[16];

            random.GetBytes(sessionKey);
            random.GetBytes(iv);

            (EncryptionKey key, byte[] encryptedData) encryptedResult = hybridEncryption.EncryptData(sessionKey, data, iv);

            var key = encryptedResult.key;

            var keyBlob = key.ExportToBlob();

            var keyFromBlob = EncryptionKey.FromBlob(keyBlob);
            var rsaEcryption = RSAEncryption.FromExistingContainer("somecontainer");

            var decryptedSessionKey = rsaEcryption.DecryptData(keyFromBlob.SessionKey);

            Assert.That(sessionKey, Is.EqualTo(decryptedSessionKey));
        }
    }
}