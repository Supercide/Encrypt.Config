using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using Encrypt.Config.ConsoleHost;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.RSA;
using Encrypt.Config.Encryption.Symmetric;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests {
    public class GivenApplication_WhenEncryptingFile
    {
        private readonly string _currentUser;
        private readonly string[] _files;

        public GivenApplication_WhenEncryptingFile()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            _files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys")
                              .Concat(Directory.EnumerateFiles(Directory.GetCurrentDirectory()))
                              .ToArray();

            _currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[]{"create", "keys",
                $"-{WellKnownCommandArguments.USERNAME}", $"{_currentUser}",
                $"-{WellKnownCommandArguments.EXPORT_KEY}", "key",
                $"-{WellKnownCommandArguments.EXPORT_PUBLIC_KEY}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", "TestContainer"});

            Program.Main(new[]{"encrypt",
                $"-{WellKnownCommandArguments.IMPORT_KEY}", "key",
                $"-{WellKnownCommandArguments.FILE_PATH}", "appsettings.json",
                $"-{WellKnownCommandArguments.ENCRYPTED_FILE_OUT}", "encryptedSettings"});
        }

        [Test]
        public void ThenCreatesEncryptedFileAtOutLocation()
        {
            FileAssert.Exists("encryptedSettings");
        }

        [Test]
        public void ThenEncryptsFile()
        {
            Assert.That(File.ReadAllBytes("applicationSettings.json"), Is.Not.EqualTo(File.ReadAllBytes("encryptedSettings")));
        }

        [Test]
        public void ThenEncryptedFileCanBeDecryptedWithKey()
        {
            var encryptedKey = EncryptionKey.FromBlob(File.ReadAllBytes("decryptionkey"));

            var encryptedData = File.ReadAllBytes("encryptedSettings");

            HybridEncryption hybridEncryption = new HybridEncryption(new RSAEncryption("TestContainer"), new AESEncryption());

            var data = hybridEncryption.DecryptData(encryptedKey, encryptedData);

            Assert.That(encryptedData, Is.EqualTo(data));
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            if (File.Exists("publicKey.xml"))
            {
                File.Delete("publicKey.xml");
            }

            var files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys")
                                 .Concat(Directory.EnumerateFiles(Directory.GetCurrentDirectory()));

            var newFiles = files.Except(_files);

            foreach (var newFile in newFiles)
            {
                File.Delete(newFile);
            }
        }
    }
}