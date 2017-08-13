using System;
using System.IO;
using System.Security.Principal;
using Encrypt.Config.ConsoleHost;
using Encrypt.Config.ConsoleHost.Constants;
using Encrypt.Config.ConsoleHost.Exceptions;
using Encrypt.Config.Encryption;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.Hybrid;
using Encrypt.Config.Encryption.Symmetric;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests.Console {
    public class EncryptionTests
    {
        private readonly string _currentUser = WindowsIdentity.GetCurrent().Name;
        private readonly string _keyFile = $"{Guid.NewGuid()}";
        private readonly string _containerName = $"{Guid.NewGuid()}";

        public EncryptionTests()
        {
            Program.Main(new[]{"create",
                $"-{WellKnownCommandArguments.USERNAME}", _currentUser,
                $"-{WellKnownCommandArguments.EXPORT_KEY}", _keyFile,
                $"-{WellKnownCommandArguments.KEY_TYPE_PUBLIC}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", _containerName});
        }

        [Test]
        public void WhenEncryptingFile_WithFileOutArgument_ThenCreatesEncryptedFileAtOutLocation()
        {
            var encryptedFile = $"{Guid.NewGuid()}";
            
            Program.Main(new[]{"encrypt",
                $"-{WellKnownCommandArguments.IMPORT_KEY}", _keyFile,
                $"-{WellKnownCommandArguments.FILE_PATH}", "appsettings.json",
                $"-{WellKnownCommandArguments.ENCRYPTED_FILE_OUT}", encryptedFile});

            FileAssert.Exists(encryptedFile);
        }

        [Test]
        public void WhenEncryptingFile_WithMissingImportKeyArgument_ThenThrowsMissingKeyException()
        {
            var encryptedFile = $"{Guid.NewGuid()}";

            Assert.Throws<MissingKeyException>(() =>
            {
                Program.Main(new[]{"encrypt",
                    $"-{WellKnownCommandArguments.FILE_PATH}", "appsettings.json",
                    $"-{WellKnownCommandArguments.ENCRYPTED_FILE_OUT}", encryptedFile});
            });
        }

        [Test]
        public void WhenEncryptingFile_WithMissingFilePathArgument_ThenThrowsMissingFilePathException()
        {
            var encryptedFile = $"{Guid.NewGuid()}";

            Assert.Throws<MissingFilePathException>(() =>
            {
                Program.Main(new[]{"encrypt",
                    $"-{WellKnownCommandArguments.IMPORT_KEY}", _keyFile,
                    $"-{WellKnownCommandArguments.ENCRYPTED_FILE_OUT}", encryptedFile});
            });
        }

        [Test]
        public void WhenEncryptingFile_WithMissingFileOutArgument_ThenThrowsMissingFilePathException()
        {
            Assert.Throws<MissingFilePathException>(() =>
            {
                Program.Main(new[]{"encrypt",
                    $"-{WellKnownCommandArguments.IMPORT_KEY}", _keyFile,
                    $"-{WellKnownCommandArguments.FILE_PATH}", "appsettings.json",
                });
            });
        }

        [Test]
        public void ThenEncryptsFile()
        {
            var encryptedFile = $"{Guid.NewGuid()}";

            Program.Main(new[]{"encrypt",
                $"-{WellKnownCommandArguments.IMPORT_KEY}", _keyFile,
                $"-{WellKnownCommandArguments.FILE_PATH}", "appsettings.json",
                $"-{WellKnownCommandArguments.ENCRYPTED_FILE_OUT}", encryptedFile});

            Assert.That(File.ReadAllBytes("appsettings.json"), Is.Not.EqualTo(File.ReadAllBytes(encryptedFile)));
        }

        [Test]
        public void ThenEncryptedFileCanBeDecryptedWithKey()
        {
            var encryptedFile = $"{Guid.NewGuid()}";

            Program.Main(new[]{"encrypt",
                $"-{WellKnownCommandArguments.IMPORT_KEY}", _keyFile,
                $"-{WellKnownCommandArguments.FILE_PATH}", "appsettings.json",
                $"-{WellKnownCommandArguments.ENCRYPTED_FILE_OUT}", encryptedFile});

            var expectedFile = File.ReadAllBytes("appsettings.json");

            var encryptedKey = EncryptionKey.FromBlob(File.ReadAllBytes("decryptionkey"));

            var encryptedData = File.ReadAllBytes(encryptedFile);

            HybridEncryption hybridEncryption = new HybridEncryption(new RSAEncryption(_containerName), new AESEncryption());

            var decryptedFile = hybridEncryption.DecryptData(encryptedKey, encryptedData);
            
            Assert.That(expectedFile, Is.EqualTo(decryptedFile));
        }
    }
}