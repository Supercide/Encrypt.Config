using System.Security.Cryptography;
using System.Security.Principal;
using Encrypt.Config.Encryption;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.Hybrid;
using Encrypt.Config.Encryption.Symmetric;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests.Encryption.HybridEncryptTests
{
    public class WhenDecryptingData
    {
        [Test]
        public void GivenEncryptingData_WhenDecryptingData_ThenDataIsDecrypted()
        {
            var currentUser = WindowsIdentity.GetCurrent()
                                             .Name;

            var target = new RSAEncryption("target", currentUser);
            var signatureContainer = new RSAEncryption("signatureContainer", currentUser);
            var signaturePublicKey = signatureContainer.ExportKey(false);
            var targetPublicKey = target.ExportKey(false);

            HybridEncryption hybridEncryption = HybridEncryption.CreateEncryption(targetPublicKey, "signatureContainer");
            HybridDecryption hybridDecryption = HybridDecryption.CreateDecryption("target", signaturePublicKey);

            RandomNumberGenerator random = new RNGCryptoServiceProvider();

            var data = new byte[512];
            var sessionKey = new byte[32];
            var iv = new byte[16];

            random.GetBytes(sessionKey);
            random.GetBytes(iv);
            random.GetBytes(data);

            (EncryptionSettings key, byte[] encryptedData) encryptedResult = hybridEncryption.EncryptData(sessionKey, data, iv);



            var decryptedData = hybridDecryption.DecryptData(encryptedResult.key, encryptedResult.encryptedData);

            Assert.That(decryptedData, Is.EqualTo(data));
        }

        [Test]
        public void GivenEncryptingData_WhenDecryptingData_FromImportedKey_ThenDataIsDecrypted()
        {
            var currentUser = WindowsIdentity.GetCurrent()
                                             .Name;

            var target = new RSAEncryption("target", currentUser);
            var signatureContainer = new RSAEncryption("signatureContainer", currentUser);
            var signaturePublicKey = signatureContainer.ExportKey(false);
            var targetPublicKey = target.ExportKey(false);

            HybridEncryption hybridEncryption = HybridEncryption.CreateEncryption(targetPublicKey, "signatureContainer");
            HybridDecryption hybridDecryption = HybridDecryption.CreateDecryption("target", signaturePublicKey);

            RandomNumberGenerator random = new RNGCryptoServiceProvider();

            var data = new byte[512];
            var sessionKey = new byte[32];
            var iv = new byte[16];

            random.GetBytes(sessionKey);
            random.GetBytes(iv);
            random.GetBytes(data);

            (EncryptionSettings key, byte[] encryptedData) encryptedResult = hybridEncryption.EncryptData(sessionKey, data, iv);

            var keyBlob = encryptedResult.key.ExportToBlob();

            var keyFromBlob = EncryptionSettings.FromBlob(keyBlob);

            var decryptedData = hybridDecryption.DecryptData(keyFromBlob, encryptedResult.encryptedData);

            Assert.That(decryptedData, Is.EqualTo(data));
        }
    }
}
