using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using Encrypt.Config.ConsoleHost;
using Encrypt.Config.ConsoleHost.Constants;
using Encrypt.Config.Encryption;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.Hybrid;
using Encrypt.Config.Encryption.Symmetric;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests {
    public class GivenApplication_WhenEncryptingFile
    {
        private readonly string _currentUser;
        private string _keyFile;
        private string _containerName;
        private string _encryptedFile;

        public GivenApplication_WhenEncryptingFile()
        {
           _currentUser = WindowsIdentity.GetCurrent().Name;

            _keyFile = $"{Guid.NewGuid()}";
            _containerName = $"{Guid.NewGuid()}";
            _encryptedFile = $"{Guid.NewGuid()}";
            Program.Main(new[]{"create", "keys",
                $"-{WellKnownCommandArguments.USERNAME}", $"{_currentUser}",
                $"-{WellKnownCommandArguments.EXPORT_KEY}", _keyFile,
                $"-{WellKnownCommandArguments.EXPORT_PUBLIC_KEY}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", _containerName});

            Program.Main(new[]{"encrypt",
                $"-{WellKnownCommandArguments.IMPORT_KEY}", _keyFile,
                $"-{WellKnownCommandArguments.FILE_PATH}", "appsettings.json",
                $"-{WellKnownCommandArguments.ENCRYPTED_FILE_OUT}", _encryptedFile});
        }

        [Test]
        public void ThenCreatesEncryptedFileAtOutLocation()
        {
            FileAssert.Exists(_encryptedFile);
        }

        [Test]
        public void ThenEncryptsFile()
        {
            Assert.That(File.ReadAllBytes("appsettings.json"), Is.Not.EqualTo(File.ReadAllBytes(_encryptedFile)));
        }

        [Test]
        public void ThenEncryptedFileCanBeDecryptedWithKey()
        {
            var expectedFile = File.ReadAllBytes("appsettings.json");

            var encryptedKey = EncryptionKey.FromBlob(File.ReadAllBytes("decryptionkey"));

            var encryptedData = File.ReadAllBytes(_encryptedFile);

            HybridEncryption hybridEncryption = new HybridEncryption(new RSAEncryption(_containerName), new AESEncryption());

            var decryptedFile = hybridEncryption.DecryptData(encryptedKey, encryptedData);
            
            Assert.That(expectedFile, Is.EqualTo(decryptedFile));
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            if (File.Exists("publicKey.xml"))
            {
                File.Delete("publicKey.xml");
            }
        }
    }
}