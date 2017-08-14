using System.Security.Cryptography;
using System.Security.Principal;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.Hybrid;
using Encrypt.Config.Encryption.Symmetric;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests.Encryption.HybridEncryptTests
{
    public class WhenEncryptingData
    {
        [Test]
        public void GivenRandomData_WhenEncryptingData_ThenDataIsEncrypted()
        {
            var currentUser = WindowsIdentity.GetCurrent()
                                             .Name;

            var target = new RSAEncryption("target", currentUser);
            new RSAEncryption("signatureContainer", currentUser);
            var targetPublicKey = target.ExportKey(false);

            HybridEncryption hybridEncryption = HybridEncryption.CreateEncryption(targetPublicKey, "signatureContainer");

            RandomNumberGenerator random = new RNGCryptoServiceProvider();

            var data = new byte[512];
            var sessionKey = new byte[32];
            var iv = new byte[16];

            random.GetBytes(sessionKey);
            random.GetBytes(iv);
            random.GetBytes(data);

            var encryptedResult = hybridEncryption.EncryptData(sessionKey, data, iv);

            Assert.That(encryptedResult.encryptedData, Is.Not.EqualTo(data));
        }

        [Test]
        public void GivenRandomData_WhenEncryptingData_ThenIVIsReturned()
        {
            var currentUser = WindowsIdentity.GetCurrent()
                                             .Name;

            var target = new RSAEncryption("target", currentUser);
            new RSAEncryption("signatureContainer", currentUser);
            var targetPublicKey = target.ExportKey(false);

            HybridEncryption hybridEncryption = HybridEncryption.CreateEncryption(targetPublicKey, "signatureContainer");

            RandomNumberGenerator random = new RNGCryptoServiceProvider();

            var data = new byte[512];
            var sessionKey = new byte[32];
            var iv = new byte[16];

            random.GetBytes(sessionKey);
            random.GetBytes(iv);
            random.GetBytes(data);

            var encryptedResult = hybridEncryption.EncryptData(sessionKey, data, iv);

            Assert.That(encryptedResult.key.IV, Is.Not.Empty);
        }

        [Test]
        public void GivenRandomData_WhenEncryptingData_ThenHMACHashIsReturned()
        {
            var currentUser = WindowsIdentity.GetCurrent()
                                             .Name;

            var target = new RSAEncryption("target", currentUser);
            new RSAEncryption("signatureContainer", currentUser);
            var targetPublicKey = target.ExportKey(false);

            HybridEncryption hybridEncryption = HybridEncryption.CreateEncryption(targetPublicKey, "signatureContainer");

            RandomNumberGenerator random = new RNGCryptoServiceProvider();

            var data = new byte[512];
            var sessionKey = new byte[32];
            var iv = new byte[16];

            random.GetBytes(sessionKey);
            random.GetBytes(iv);
            random.GetBytes(data);

            var encryptedResult = hybridEncryption.EncryptData(sessionKey, data, iv);

            Assert.That(encryptedResult.key.HMACHash, Is.Not.Empty);
        }

        [Test]
        public void GivenRandomData_WhenEncryptingData_ThenSessionKeyIsReturned()
        {
            var currentUser = WindowsIdentity.GetCurrent()
                                             .Name;

            var target = new RSAEncryption("target", currentUser);
            new RSAEncryption("signatureContainer", currentUser);
            var targetPublicKey = target.ExportKey(false);

            HybridEncryption hybridEncryption = HybridEncryption.CreateEncryption(targetPublicKey, "signatureContainer");

            RandomNumberGenerator random = new RNGCryptoServiceProvider();

            var data = new byte[512];
            var sessionKey = new byte[32];
            var iv = new byte[16];

            random.GetBytes(sessionKey);
            random.GetBytes(iv);
            random.GetBytes(data);

            var encryptedResult = hybridEncryption.EncryptData(sessionKey, data, iv);

            Assert.That(encryptedResult.key.SessionKey, Is.Not.Empty);
        }
    }
}
