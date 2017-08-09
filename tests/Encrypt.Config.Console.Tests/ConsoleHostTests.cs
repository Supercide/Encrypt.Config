using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using Encrypt.Config.ConsoleHost;
using Encrypt.Config.Json;
using Encrypt.Config.RSA;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests
{
    //TODO:Help command

    [TestFixture]
    public class ConsoleHostTests
    {
        private readonly string[] _files;

        public ConsoleHostTests()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            _files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys")
                              .Concat(Directory.EnumerateFiles(Directory.GetCurrentDirectory()))
                              .ToArray();
        }

        [Test]
        public void GivenCreateKeysCommand_WithKeyOutArgumentSupplied_WhenCreatingKeys_ThenExportsPublicKey()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new []{"create", "keys",
                                $"-{WellKnownCommandArguments.USERNAME}", $"{currentUser}",
                                $"-{WellKnownCommandArguments.EXPORT_KEY}", "publicKey.xml",
                                $"-{WellKnownCommandArguments.EXPORT_PUBLIC_KEY}",
                                $"-{WellKnownCommandArguments.CONTAINER_NAME}", "TestContainer"});

            FileAssert.Exists("publicKey.xml");
        }

        [Test]
        public void GivenCreateKeysCommand_WithNoOptionalArguments_WhenCreatingKeys_ThenCreatesKeyForUsername()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[] { "create", "keys",
                                $"-{WellKnownCommandArguments.USERNAME}", $"{currentUser}",
                                $"-{WellKnownCommandArguments.CONTAINER_NAME}", "TestContainer"});

            var files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys");

            var containerFile = files.Except(_files).Single();
            
            var controlList = File.GetAccessControl(containerFile);

            var accessRule = controlList.GetAccessRules(true, true, typeof(NTAccount))
                                         .Cast<AuthorizationRule>()
                                         .Single();

            Assert.That(accessRule.IdentityReference.Value, Is.EqualTo(currentUser));
        }

        [Test]
        public void GivenEncryptJsonConfigCommand_WithImportPublicKey_WhenEncryptingConfig_ThenEncryptsConfigWithPublicKey()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[] { "create", "keys",
                $"-{WellKnownCommandArguments.USERNAME}", $"{currentUser}",
                $"-{WellKnownCommandArguments.EXPORT_KEY}", "publicKey.xml",
                $"-{WellKnownCommandArguments.EXPORT_PUBLIC_KEY}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", "TestContainer"});

            Program.Main(new[] { "encrypt","json","config",
                $"-{WellKnownCommandArguments.IMPORT_KEY}", "publicKey.xml",
                $"-{WellKnownCommandArguments.JSON_CONFIGURATION_PATH}", "appsettings.json"});

            var decryptedConfig = DecryptFile("TestContainer");

            FileAssert.Exists("appsettings.encrypted.json");

            Assert.That(decryptedConfig.ToString(), Is.EqualTo(JObject.Parse(File.ReadAllText("appsettings.json")).ToString()));
        }

        [Test]
        public void GivenCreateContainerCommand_WithKeyContainingPublicAndPrivateKeys_WhenCreatingContainer_ThenCreatesContainer_WithPublicAndPrivateKeys()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;
            var expectedKeyFile = "expectedKey.xml";

            Program.Main(new[]{"create", "keys",
                $"-{WellKnownCommandArguments.USERNAME}", $"{currentUser}",
                $"-{WellKnownCommandArguments.EXPORT_KEY}", expectedKeyFile,
                $"-{WellKnownCommandArguments.EXPORT_PRIVATE_KEY}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", "anyContainerName"});

            var containerWithImportedKeys = "ContainerWithImportedKeys";

            Program.Main(new[]{"create", "container",
                $"-{WellKnownCommandArguments.USERNAME}", $"{currentUser}",
                $"-{WellKnownCommandArguments.IMPORT_KEY}", expectedKeyFile,
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", containerWithImportedKeys});

            var provider = LoadContainer(containerWithImportedKeys);

            var actualImportedKey = provider.ToXmlString(true);
            var expectedKey = File.ReadAllText(expectedKeyFile);

            Assert.That(actualImportedKey, Is.EqualTo(expectedKey));
        }

        [Test]
        public void GivenCreateContainerCommand_WithKeyOnlyPublicKey_WhenCreatingContainer_ThenThrowsException()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;
            var expectedKeyFile = "expectedKey.xml";

            Program.Main(new[]{"create", "keys",
                $"-{WellKnownCommandArguments.USERNAME}", $"{currentUser}",
                $"-{WellKnownCommandArguments.EXPORT_KEY}", expectedKeyFile,
                $"-{WellKnownCommandArguments.EXPORT_PUBLIC_KEY}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", "anyContainerName"});


           Assert.Throws<InvalidOperationException>(() => Program.Main(new[]{"create", "container",
                $"-{WellKnownCommandArguments.USERNAME}", $"{currentUser}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", "anotherContainer",
                $"-{WellKnownCommandArguments.IMPORT_KEY}", expectedKeyFile}));
        }


        public JObject DecryptFile(string containerName)
        {
            var provider = LoadContainer(containerName);

            using (RSAWrapper wrapper = new RSAWrapper(provider))
            {
                JsonConfigurationFileEncrypter encrypter = new JsonConfigurationFileEncrypter(wrapper);

                return encrypter.Decrypt(JObject.Parse(File.ReadAllText("appsettings.encrypted.json")));
            }
        }

        private static RSACryptoServiceProvider LoadContainer(string containerName)
        {
            CspParameters cspParams = new CspParameters
            {
                KeyContainerName = containerName,
                KeyNumber = (int) KeyNumber.Exchange,
                Flags = CspProviderFlags.UseMachineKeyStore | CspProviderFlags.NoPrompt,
            };

            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(cspParams);
            return provider;
        }

        [Test]
        public void GivenCreateKeysCommand_WithInvalidArguments_WhenCreatingKeys_ThenIgnoresAdditionalArguments()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[]{"create", "keys",
                $"-{WellKnownCommandArguments.USERNAME}", $"{currentUser}",
                $"-{WellKnownCommandArguments.EXPORT_KEY}", "key.xml",
                $"-{WellKnownCommandArguments.JSON_CONFIGURATION_PATH}", "appsettings.json",
                $"-{WellKnownCommandArguments.EXPORT_PUBLIC_KEY}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", "anyContainerName"});

            FileAssert.DoesNotExist("appsettings.encrypted.json");
        }

        [TearDown]
        public void CleanUp()
        {
            if(File.Exists("publicKey.xml"))
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
