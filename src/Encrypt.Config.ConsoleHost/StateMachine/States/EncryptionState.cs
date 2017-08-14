using System.IO;
using Encrypt.Config.ConsoleHost.Constants;
using Encrypt.Config.ConsoleHost.Exceptions;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.Hybrid;
using Encrypt.Config.Encryption.NumberGenerators;
using Encrypt.Config.Encryption.Symmetric;

namespace Encrypt.Config.ConsoleHost.StateMachine.States {
    public class EncryptionState : ConsoleState
    {
        public override void Handle(Context context)
        {
            if(!context.Arguments.TryGetValue(WellKnownCommandArguments.FILE_PATH, out var filePath))
            {
                throw new MissingFilePathException("Missing file path argument. try encrypt --help for more information");
            }

            if(!context.Arguments.TryGetValue(WellKnownCommandArguments.IMPORT_KEY, out var publicKeyPath))
            {
                throw new MissingKeyException("Missing key argument. try encrypt --help for more information");
            }

            if(!context.Arguments.TryGetValue(WellKnownCommandArguments.ENCRYPTED_FILE_OUT, out var encryptedFilePath))
            {
                throw new MissingFilePathException("Missing encrypted file path argument. try encrypt --help for more information");
            }

            if (!context.Arguments.TryGetValue(WellKnownCommandArguments.SIGNATURE_CONTAINER, out var signatureContainer))
            {
                throw new ContainerNameMissingException("Missing name of signature container. try encrypt --help for more information");
            }

            var publicKey = File.ReadAllText(publicKeyPath);

            var fileEncrypter = new FileEncrypter(HybridEncryption.CreateEncryption(publicKey,signatureContainer), 
                                                  new RNGCryptoRandomBytesGenerator());

            var encryptionResult = fileEncrypter.Encrypt(filePath);

            File.WriteAllBytes(encryptedFilePath, encryptionResult.data);

            var keyPath = new FileInfo(encryptedFilePath).Directory.FullName;

            File.WriteAllBytes($"{keyPath}/decryptionkey", encryptionResult.key.ExportToBlob());

            SetEndState(context);
        }
    }
}