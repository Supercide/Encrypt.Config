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

            var target = new RSAEncryption("target", currentUser);
            new RSAEncryption("signatureContainer", currentUser);
            var targetPublicKey = target.ExportKey(false);

            HybridEncryption hybridEncryption = HybridEncryption.CreateEncryption(targetPublicKey, "signatureContainer");

            RandomNumberGenerator random = new RNGCryptoServiceProvider();

            var data = File.ReadAllBytes("appsettings.json");
            var sessionKey = new byte[32];
            var iv = new byte[16];

            random.GetBytes(sessionKey);
            random.GetBytes(iv);

            (EncryptionSettings key, byte[] encryptedData) encryptedResult = hybridEncryption.EncryptData(sessionKey, data, iv);

            var key = encryptedResult.key;

            var keyBlob = key.ExportToBlob();

            var keyFromBlob = EncryptionSettings.FromBlob(keyBlob);

            Assert.That(keyFromBlob.IV, Is.EqualTo(key.IV));
        }

        [Test]
        public void GivenEncryptionKeyBlob_WhenImportingKey_ThenImportsHMACHashCorrectly()
        {
            var currentUser = WindowsIdentity.GetCurrent()
                                             .Name;

            var target = new RSAEncryption("target", currentUser);
            new RSAEncryption("signatureContainer", currentUser);
            var targetPublicKey = target.ExportKey(false);

            HybridEncryption hybridEncryption = HybridEncryption.CreateEncryption(targetPublicKey, "signatureContainer");

            RandomNumberGenerator random = new RNGCryptoServiceProvider();

            var data =File.ReadAllBytes("appsettings.json");
            var sessionKey = new byte[32];
            var iv = new byte[16];

            random.GetBytes(sessionKey);
            random.GetBytes(iv);

            (EncryptionSettings key, byte[] encryptedData) encryptedResult = hybridEncryption.EncryptData(sessionKey, data, iv);

            var key = encryptedResult.key;

            var keyBlob = key.ExportToBlob();

            var keyFromBlob = EncryptionSettings.FromBlob(keyBlob);

            Assert.That(keyFromBlob.HMACHash, Is.EqualTo(key.HMACHash));
        }

        [Test]
        public void GivenEncryptionKeyBlob_WhenImportingKey_ThenImportsSessionKeyCorrectly()
        {
            var currentUser = WindowsIdentity.GetCurrent()
                                             .Name;

            var target = new RSAEncryption("target", currentUser);
            new RSAEncryption("signatureContainer", currentUser);
            var targetPublicKey = target.ExportKey(false);

            HybridEncryption hybridEncryption = HybridEncryption.CreateEncryption(targetPublicKey, "signatureContainer");

            RandomNumberGenerator random = new RNGCryptoServiceProvider();

            var data = File.ReadAllBytes("appsettings.json");
            var sessionKey = new byte[32];
            var iv = new byte[16];

            random.GetBytes(sessionKey);
            random.GetBytes(iv);

            (EncryptionSettings key, byte[] encryptedData) encryptedResult = hybridEncryption.EncryptData(sessionKey, data, iv);

            var key = encryptedResult.key;

            var keyBlob = key.ExportToBlob();

            var keyFromBlob = EncryptionSettings.FromBlob(keyBlob);

            Assert.That(keyFromBlob.SessionKey, Is.EqualTo(key.SessionKey));
        }

        [Test]
        public void GivenEncryptionKeyBlob_WhenImportingKey_ThenDecryptsSessionKeyCorrectly()
        {
            var currentUser = WindowsIdentity.GetCurrent()
                                             .Name;

            var target = new RSAEncryption("target", currentUser);
            new RSAEncryption("signatureContainer", currentUser);

            var targetPublicKey = target.ExportKey(false);

            HybridEncryption hybridEncryption = HybridEncryption.CreateEncryption(targetPublicKey, "signatureContainer");

            RandomNumberGenerator random = new RNGCryptoServiceProvider();

            var data = File.ReadAllBytes("appsettings.json");
            var sessionKey = new byte[32];
            var iv = new byte[16];

            random.GetBytes(sessionKey);
            random.GetBytes(iv);

            (EncryptionSettings key, byte[] encryptedData) encryptedResult = hybridEncryption.EncryptData(sessionKey, data, iv);

            var key = encryptedResult.key;

            var keyBlob = key.ExportToBlob();

            var keyFromBlob = EncryptionSettings.FromBlob(keyBlob);
            var rsaEcryption = RSAEncryption.FromExistingContainer("target");

            var decryptedSessionKey = rsaEcryption.DecryptData(keyFromBlob.SessionKey);

            Assert.That(sessionKey, Is.EqualTo(decryptedSessionKey));
        }
    }
}