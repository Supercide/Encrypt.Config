using System.Security.Cryptography;
using System.Security.Principal;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.RSA;
using Encrypt.Config.Encryption.Symmetric;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests.Encryption.HybridEncryptTests
{
    public class WhenExportingKey
    {
        private string _currentUser;

        public WhenExportingKey()
        {
            
        }

        [Test]
        public void GWT()
        {
            var currentUser = WindowsIdentity.GetCurrent()
                                             .Name;

            HybridEncryption hybridEncryption = new HybridEncryption(new RSAEncryption("somecontainer", currentUser), new AESEncryption());
            
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
    }
}
