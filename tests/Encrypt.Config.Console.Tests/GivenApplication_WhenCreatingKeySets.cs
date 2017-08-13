using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading.Tasks;
using Encrypt.Config.ConsoleHost;
using Encrypt.Config.Encryption.Random;
using Encrypt.Config.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests
{
    [TestFixture]
    public class GivenApplication_WhenCreatingKeySets
    {
        private readonly string _currentUser;

        readonly string containerName = $"{Guid.NewGuid()}";


        public GivenApplication_WhenCreatingKeySets()
        {
            _currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[]{"create", "keys",
                $"-{WellKnownCommandArguments.USERNAME}", $"{_currentUser}",
                $"-{WellKnownCommandArguments.EXPORT_KEY}", "key",
                $"-{WellKnownCommandArguments.EXPORT_PUBLIC_KEY}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", containerName});
        }

        [Test]
        public void ThenExportsPublicKey()
        {
            FileAssert.Exists("key");
        }

        [Test]
        public async Task ThenSetsACLOnKeyContainer()
        {
            var cspParameters = new CspParameters
            {
                KeyContainerName = containerName,
                Flags = CspProviderFlags.UseMachineKeyStore
            };

            CspKeyContainerInfo info = new CspKeyContainerInfo(cspParameters);

            var containerPath = Path.Combine(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys", info.UniqueKeyContainerName);

            var controlList = File.GetAccessControl(containerPath);

            var accessRule = controlList.GetAccessRules(true, true, typeof(NTAccount))
                                         .Cast<AuthorizationRule>()
                                         .Single();

            Assert.That(accessRule.IdentityReference.Value, Is.EqualTo(_currentUser));
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            if(File.Exists("publicKey.xml"))
            {
                File.Delete("publicKey.xml");
            }
        }
    }
}
