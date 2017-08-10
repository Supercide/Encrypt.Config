using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Encrypt.Config.RSA;

namespace Encrypt.Config.Console.Tests
{

    [TestFixture]
    public class RSAWrapperTests
    {
        private readonly IEnumerable<string> _files;

        readonly string User;

        public RSAWrapperTests()
        {
            User = WindowsIdentity.GetCurrent().Name;

            _files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys")
                              .ToArray();
        }

        [Test]
        public void GivenValidData_WhenCreatingContainer_ThenReturnsExportKey()
        {
            var containerName = $"{Guid.NewGuid()}";

            using (var rsaContainer = RSAContainerFactory.Create(containerName, User))
            {
                var rsaExport = rsaContainer.ToXmlString(false);

                Assert.That(rsaExport, Is.Not.Null);
            }
        }

        [Test]
        public void GivenDataEncryptedWithPublicKey_WhenDecryptingDataFromPrivateKey_ThenDecryptsData()
        {
            var salt = new byte[]{1,4,3,2,5,3,2};

            string message = "Secret message";

            using (var rsaContainerOne = RSAContainerFactory.Create($"{Guid.NewGuid()}", User))
            {
                var privateKeyExport = rsaContainerOne.ToXmlString(true);
                
                using (var privateContainer = RSAContainerFactory.CreateFromKey($"{Guid.NewGuid()}", privateKeyExport, User))
                {
                    var encryptedData = rsaContainerOne.Encrypt(Encoding.Unicode.GetBytes(message),RSAEncryptionPadding.Pkcs1, salt);

                    var decryptedData = privateContainer.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1, salt);

                    var actualMessage = Encoding.Unicode.GetString(decryptedData);

                    Assert.That(actualMessage, Is.EqualTo(message));
                }
            }
        }

        [Test]
        public void GivenUsername_WhenCreatingContainer_ThenOnlyProvidedUserNameHasAccess()
        {
            var keyContainerName = $"{Guid.NewGuid()}";

            using (RSAContainerFactory.Create(keyContainerName, User))
            {
                var container = LoadCspKeyContainerInfo(keyContainerName);

                var rule = container.CryptoKeySecurity.GetAccessRules(true, true, typeof(NTAccount))
                                    .Cast<AuthorizationRule>()
                                    .SingleOrDefault();

                Assert.That(rule, Is.Not.Null);

                Assert.That(rule.IdentityReference.Value, Is.EqualTo(User));
            }
        }

        [Test]
        public void GivenUsername_WhenCreatingContainer_ThenSetsAccessControlToReadOnlyForUser()
        {
            var containerName = $"{Guid.NewGuid()}";

            using (var rsaCryptoServiceProvider = RSAContainerFactory.Create(containerName, User))
            {
                var path = Path.Combine(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys", rsaCryptoServiceProvider.CspKeyContainerInfo.UniqueKeyContainerName);

                FileSecurity fSecurity = new FileSecurity(path, AccessControlSections.Access);

                var accessRule = fSecurity.GetAccessRules(true, true, typeof(NTAccount))
                                          .Cast<FileSystemAccessRule>()
                                          .SingleOrDefault();

                var rights = accessRule.FileSystemRights
                                       .ToString()
                                       .Split(',')
                                       .Select(x => (FileSystemRights)Enum.Parse(typeof(FileSystemRights), x, true));

                Assert.NotNull(rights);
                Assert.That(rights.Count(), Is.EqualTo(2));
                Assert.That(rights.Any(systemRights => systemRights == FileSystemRights.Read));
                Assert.That(rights.Any(systemRights => systemRights == FileSystemRights.Synchronize));
            }
        }

        [Test]
        public void GivenContainerName_WhenCreatingContainer_CreatesContainer_WithName()
        {
            var containerName = $"{Guid.NewGuid()}";
            using (var rsaCryptoServiceProvider = RSAContainerFactory.Create(containerName, User))
            {
                Assert.That(rsaCryptoServiceProvider.CspKeyContainerInfo.KeyContainerName, Is.EqualTo(containerName));
            }
        }

        private static CspKeyContainerInfo LoadCspKeyContainerInfo(string keyContainerName)
        {
            CspParameters cp = new CspParameters
            {
                KeyContainerName = keyContainerName,
                Flags = CspProviderFlags.UseMachineKeyStore
            };

            CspKeyContainerInfo container = new CspKeyContainerInfo(cp);
            return container;
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            var files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys");

            var newFiles = files.Except(_files);

            foreach (var newFile in newFiles)
            {
                File.Delete(newFile);
            }
        }
    }
}
