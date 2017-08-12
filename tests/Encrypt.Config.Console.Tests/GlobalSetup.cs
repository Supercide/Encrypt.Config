using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests
{
    [SetUpFixture]
    class GlobalSetup
    {
        private string[] _files;

        [OneTimeSetUp]
        public void SetUp()
        {
            _files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys")
                              .Concat(Directory.EnumerateFiles(Directory.GetCurrentDirectory()))
                              .ToArray();
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
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
