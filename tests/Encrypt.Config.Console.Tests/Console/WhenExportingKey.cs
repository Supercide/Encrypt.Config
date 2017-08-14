using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using Encrypt.Config.ConsoleHost;
using Encrypt.Config.ConsoleHost.Constants;
using Encrypt.Config.ConsoleHost.Exceptions;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests.Console
{
    [TestFixture]
    public class WhenExportingKey
    {
        private readonly string containerName = $"{Guid.NewGuid()}";

        private readonly string _currentUser = WindowsIdentity.GetCurrent().Name;

        public WhenExportingKey()
        {
            Program.Main(new[]{
                WellKnownCommands.CREATE_KEYS,
                $"-{WellKnownCommandArguments.USERNAME}", _currentUser,
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", containerName});
        }

        [Test]
        public void WhenExportingKey_WithKeyTypeSetToPrivate_ThenExportsPrivateKey()
        {
            var expectedKeyFile = $"{Guid.NewGuid()}";

            Program.Main(new[]{
                WellKnownCommands.EXPORT_KEY,
                $"-{WellKnownCommandArguments.EXPORT_KEY_PATH}", expectedKeyFile,
                $"-{WellKnownCommandArguments.KEY_TYPE_PRIVATE}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", containerName});

            var key = File.ReadAllText(expectedKeyFile);

            RSACryptoServiceProvider rsaCryptoService = new RSACryptoServiceProvider();
            rsaCryptoService.PersistKeyInCsp = false;
            rsaCryptoService.FromXmlString(key);

            Assert.That(rsaCryptoService.PublicOnly, Is.False);
        }

        [Test]
        public void WhenExportingKey_WithKeyTypeSetToPublic_ThenExportsPublicKey()
        {
            var expectedKeyFile = $"{Guid.NewGuid()}";

            Program.Main(new[]{
                WellKnownCommands.EXPORT_KEY,
                $"-{WellKnownCommandArguments.EXPORT_KEY_PATH}", expectedKeyFile,
                $"-{WellKnownCommandArguments.KEY_TYPE_PUBLIC}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", containerName});

            var key = File.ReadAllText(expectedKeyFile);

            RSACryptoServiceProvider rsaCryptoService = new RSACryptoServiceProvider();
            rsaCryptoService.PersistKeyInCsp = false;
            rsaCryptoService.FromXmlString(key);

            Assert.That(rsaCryptoService.PublicOnly, Is.True);
        }

        [Test]
        public void WhenExportingKey_WithMissingKeyPath_ThenThrowsMissingFilePathException()
        {
            Assert.Throws<MissingFilePathException>(() =>
            {
                Program.Main(new[]{WellKnownCommands.EXPORT_KEY,
                    $"-{WellKnownCommandArguments.KEY_TYPE_PRIVATE}",
                    $"-{WellKnownCommandArguments.CONTAINER_NAME}", containerName});
            });
        }

        [Test]
        public void WhenExportingKey_WithMissingContainerName_ThenThrowsContainerNameMissingException()
        {
            var expectedKeyFile = $"{Guid.NewGuid()}";

            Assert.Throws<ContainerNameMissingException>(() =>
            {
                Program.Main(new[]{WellKnownCommands.EXPORT_KEY,
                    $"-{WellKnownCommandArguments.EXPORT_KEY_PATH}", expectedKeyFile,
                    $"-{WellKnownCommandArguments.KEY_TYPE_PRIVATE}",
                });
            });
        }
    }
}
