using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using Encrypt.Config.Json;
using Encrypt.Config.RSA;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests
{
    
    /*
     * Help command
     */

    [TestFixture]
    public class ConsoleHostTests
    {
        /*
         * Create key | export key
         * Export key
         * encrypt file
         * 
         */
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

            Program.Main(new []{"create", "keys", "-u", $"{currentUser}","-o","publicKey.xml", "-n", "TestContainer"});

            FileAssert.Exists("publicKey.xml");
        }

        [Test]
        public void GivenCreateKeysCommand_WithNoOptionalArguments_WhenCreatingKeys_ThenCreatesKeyForUsername()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[] { "create", "keys", "-u", $"{currentUser}", "-n", "TestContainer" });

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

            Program.Main(new[] { "create", "keys", "-u", $"{currentUser}", "-pbo", "publicKey.xml", "-n", "WebMachine"});

            Program.Main(new[] { "encrypt","json","config","-u", $"{currentUser}", "-pbi", "publicKey.xml", "-n", "DeploymentMachine" , "-jc", "appsettings.json"});

            FileAssert.Exists("appsettings.encrypted.json");
        }

        [Test]
        public void GivenEncryptJsonConfigCommand_WithOnlyContainerArgument_WhenEncryptingConfig_ThenEncryptsConfigFromContainersPublicKey()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[] { "create", "keys", "-u", $"{currentUser}", "-pbo", "publicKey.xml", "-n", "WebMachine" });

            Program.Main(new[] { "create", "container", "-u", $"{currentUser}", "-pbi", "publicKey.xml", "-n", "DeploymentMachine" });

            Program.Main(new[] { "encrypt", "json", "config", "-n", "DeploymentMachine", "-jc", "appsettings.json" });

            FileAssert.Exists("appsettings.encrypted.json");
        }

        [Test]
        public void GivenCreateContainerCommand_WhenCreatingContainer_ThenCreatesContainer()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[] { "-u", $"{currentUser}", "-n", "WebMachine" });

            Assert.Fail();
        }

        
        public JObject DecryptFile(string containerName)
        {
            CspParameters cspParams = new CspParameters
            {
                KeyContainerName = containerName,
                KeyNumber = (int)KeyNumber.Exchange,
                Flags = CspProviderFlags.UseMachineKeyStore | CspProviderFlags.NoPrompt,
            };

            using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider(cspParams))
            using (RSAWrapper wrapper = new RSAWrapper(provider))
            {
                JsonConfigurationFileEncrypter encrypter = new JsonConfigurationFileEncrypter(wrapper);

                return encrypter.Decrypt(JObject.Parse(File.ReadAllText("appsettings.encrypted.json")));
            }
        }

        [Test]
        public void GivenCreateKeysCommand_WithInvalidArguments_WhenCreatingKeys_ThenThrowsException()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Assert.Throws<InvalidOperationException>(() => Program.Main(new[] { "create","keys","-u", $"{currentUser}", "-pbo", "publicKey.xml", "-n", "TestContainer", "-jc", "appsettings.json" }));
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
