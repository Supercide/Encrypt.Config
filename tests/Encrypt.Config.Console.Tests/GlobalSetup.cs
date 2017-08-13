﻿using System;
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
        public static IEnumerable<string> TrackingFiles { get; private set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            TrackingFiles = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys")
                              .Concat(Directory.EnumerateFiles(Directory.GetCurrentDirectory()))
                              .ToArray();
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            var files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys")
                                 .Concat(Directory.EnumerateFiles(Directory.GetCurrentDirectory()));

            var newFiles = files.Except(TrackingFiles);

            foreach (var newFile in newFiles)
            {
                File.Delete(newFile);
            }
        }
    }
}
