using System;
using System.IO;
using System.Security.Cryptography;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.Hybrid;
using Encrypt.Config.Encryption.NumberGenerators;
using Encrypt.Config.Encryption.Symmetric;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.States {
    public class EncryptionState : ConsoleState
    {
        public override void Handle(Context context)
        {
            try
            {
                if(!context.Arguments.TryGetValue(WellKnownCommandArguments.FILE_PATH, out var filePath))
                {
                    throw new InvalidOperationException();
                }
                
                if (!context.Arguments.TryGetValue(WellKnownCommandArguments.IMPORT_KEY, out var publicKeyPath))
                {
                    throw new InvalidOperationException();
                }

                if (!context.Arguments.TryGetValue(WellKnownCommandArguments.ENCRYPTED_FILE_OUT, out var encryptedFilePath))
                {
                    throw new InvalidOperationException();
                }

                var publicKey = File.ReadAllText(publicKeyPath);

                var fileEncrypter = new FileEncrypter(new HybridEncryption(RSAEncryption.FromPublicKey(publicKey), new AESEncryption()), new RNGCryptoRandomBytesGenerator());

                var encryptionResult = fileEncrypter.Encrypt(filePath);

                File.WriteAllBytes(encryptedFilePath, encryptionResult.data);
                
                var keyPath = new FileInfo(encryptedFilePath).Directory.FullName;
                File.WriteAllBytes($"{keyPath}/decryptionkey", encryptionResult.key.ExportToBlob());

                SetEndState(context);

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}