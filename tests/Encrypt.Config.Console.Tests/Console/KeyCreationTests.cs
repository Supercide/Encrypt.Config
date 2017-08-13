using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading.Tasks;
using Encrypt.Config.ConsoleHost;
using Encrypt.Config.ConsoleHost.Constants;
using Encrypt.Config.ConsoleHost.Exceptions;
using Encrypt.Config.Encryption.Constants;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests.Console
{
    [TestFixture]
    public class KeyCreationTests
    {
        private readonly string ExpectedKeyFile = $"{Guid.NewGuid()}";

        private readonly string _currentUser;

        public KeyCreationTests()
        {
            _currentUser = WindowsIdentity.GetCurrent().Name;
        }

        [Test]
        public void WhenCreatingKeys_WithExportKeySetToPublic_ThenExportsPublicKey()
        {
            var containerName = $"{Guid.NewGuid()}";

            Program.Main(new[]{
                WellKnownCommands.CREATE_KEYS,
                $"-{WellKnownCommandArguments.USERNAME}", _currentUser,
                $"-{WellKnownCommandArguments.EXPORT_KEY}", ExpectedKeyFile,
                $"-{WellKnownCommandArguments.KEY_TYPE_PUBLIC}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", containerName});

            var key = File.ReadAllText(ExpectedKeyFile);

            RSACryptoServiceProvider rsaCryptoService = new RSACryptoServiceProvider();
            rsaCryptoService.PersistKeyInCsp = false;
            rsaCryptoService.FromXmlString(key);

            Assert.That(rsaCryptoService.PublicOnly, Is.True);
        }

        [Test]
        public void WhenCreatingKeys_WithExportKeySetToPrivate_ThenExportsPrivateKey()
        {
            var containerName = $"{Guid.NewGuid()}";

            Program.Main(new[]{
                WellKnownCommands.CREATE_KEYS,
                $"-{WellKnownCommandArguments.USERNAME}", _currentUser,
                $"-{WellKnownCommandArguments.EXPORT_KEY}", ExpectedKeyFile,
                $"-{WellKnownCommandArguments.KEY_TYPE_PRIVATE}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", containerName});

            var key = File.ReadAllText(ExpectedKeyFile);

            RSACryptoServiceProvider rsaCryptoService = new RSACryptoServiceProvider();
            rsaCryptoService.PersistKeyInCsp = false;
            rsaCryptoService.FromXmlString(key);

            Assert.That(rsaCryptoService.PublicOnly, Is.False);
        }

        [Test]
        public void WhenCreatingKeys_WithUsernameArgument_ThenSetsACLOnKeyContainerForUser()
        {
            var containerName = $"{Guid.NewGuid()}";

            Program.Main(new[]{
                WellKnownCommands.CREATE_KEYS,
                $"-{WellKnownCommandArguments.USERNAME}", _currentUser,
                $"-{WellKnownCommandArguments.EXPORT_KEY}", ExpectedKeyFile,
                $"-{WellKnownCommandArguments.KEY_TYPE_PUBLIC}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", containerName});

            var cspParameters = new CspParameters
            {
                KeyContainerName = containerName,
                Flags = CspProviderFlags.UseMachineKeyStore
            };

            CspKeyContainerInfo info = new CspKeyContainerInfo(cspParameters);

            var containerPath = Path.Combine(WellKnownPaths.RSA_MACHINEKEYS, info.UniqueKeyContainerName);

            var controlList = File.GetAccessControl(containerPath);

            var accessRule = controlList.GetAccessRules(true, true, typeof(NTAccount))
                                         .Cast<AuthorizationRule>()
                                         .Single();

            Assert.That(accessRule.IdentityReference.Value, Is.EqualTo(_currentUser));
        }

        [Test]
        public void WhenCreatingKeys_WithMissingUserName_ThenThrowsUsernameMissingException()
        {
            var containerName = $"{Guid.NewGuid()}";

            Assert.Throws<UsernameMissingException>(() =>
            {
                Program.Main(new[]
                {
                    WellKnownCommands.CREATE_KEYS,
                    $"-{WellKnownCommandArguments.CONTAINER_NAME}", containerName,
                    $"-{WellKnownCommandArguments.EXPORT_KEY}", ExpectedKeyFile,
                    $"-{WellKnownCommandArguments.KEY_TYPE_PRIVATE}",
                });
            });
        }

        [Test]
        public void WhenCreatingKeys_WithMissingContainerName_ThenThrowsContainerNameMissingException()
        {
            Assert.Throws<ContainerNameMissingException>(() =>
            {
                Program.Main(new[]
                {
                    WellKnownCommands.CREATE_KEYS,
                    $"-{WellKnownCommandArguments.USERNAME}", _currentUser,
                    $"-{WellKnownCommandArguments.EXPORT_KEY}", ExpectedKeyFile,
                    $"-{WellKnownCommandArguments.KEY_TYPE_PUBLIC}"
                });
            });
        }

        [Test]
        public void WhenCreatingKeys_WithMissingKeyType_ThenDefaultsToExportingPublicKey()
        {
            var containerName = $"{Guid.NewGuid()}";

            Program.Main(new[]{
                WellKnownCommands.CREATE_KEYS,
                $"-{WellKnownCommandArguments.USERNAME}", _currentUser,
                $"-{WellKnownCommandArguments.EXPORT_KEY}", ExpectedKeyFile,
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", containerName});

            var key = File.ReadAllText(ExpectedKeyFile);

            RSACryptoServiceProvider rsaCryptoService = new RSACryptoServiceProvider();
            rsaCryptoService.PersistKeyInCsp = false;
            rsaCryptoService.FromXmlString(key);

            Assert.That(rsaCryptoService.PublicOnly, Is.True);
        }

        [TearDown]
        public void CleanUp()
        {
            if(File.Exists(ExpectedKeyFile))
            {
                File.Delete(ExpectedKeyFile);
            }
        }
    }
}
