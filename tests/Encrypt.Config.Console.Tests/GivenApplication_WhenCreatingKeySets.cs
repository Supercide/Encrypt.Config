using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
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
        private readonly string[] _files;
        private readonly string _currentUser;

        public GivenApplication_WhenCreatingKeySets()
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
        }

        [Test]
        public void ThenExportsPublicKey()
        {
            FileAssert.Exists("key");
        }

        [Test]
        public void ThenSetsACLOnKeyContainer()
        {
            var files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys");

            var containerFile = files.Except(_files).Single();

            var controlList = File.GetAccessControl(containerFile);

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
