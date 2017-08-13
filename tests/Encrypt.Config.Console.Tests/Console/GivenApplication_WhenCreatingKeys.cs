using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading.Tasks;
using Encrypt.Config.ConsoleHost;
using Encrypt.Config.ConsoleHost.Constants;
using Encrypt.Config.Encryption.Constants;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests.Console
{
    [TestFixture]
    public class GivenApplication_WhenCreatingKeys
    {
        private readonly string ExpectedKeyFile = $"{Guid.NewGuid()}";

        private readonly string _currentUser;

        readonly string containerName = $"{Guid.NewGuid()}";


        public GivenApplication_WhenCreatingKeys()
        {
            _currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[]{
                WellKnownCommands.CREATE_KEYS,
                $"-{WellKnownCommandArguments.USERNAME}", _currentUser,
                $"-{WellKnownCommandArguments.EXPORT_KEY}", ExpectedKeyFile,
                $"-{WellKnownCommandArguments.EXPORT_PUBLIC_KEY}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", containerName});
        }

        [Test]
        public void ThenExportsPublicKey()
        {
            FileAssert.Exists(ExpectedKeyFile);
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

            var containerPath = Path.Combine(WellKnownPaths.RSA_MACHINEKEYS, info.UniqueKeyContainerName);

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
