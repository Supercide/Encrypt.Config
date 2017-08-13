# Encrypt.Config
[![Build status](https://ci.appveyor.com/api/projects/status/gfh5l8uq94ws732x/branch/master?svg=true)](https://github.com/Supercide/Encrypt.Config/tree/master)

JSON Configuration encryption tool
## Table of contents
- [Read Me](./readme.md)
- [Architecture](./docs/Architecture.md)
- Instruction Manual
    - [Creating Keys](./docs/CreatingKeys.md)
    - [Exporting Keys](./docs/ExportingKeys.md)
    - [Encrypting](./docs/Encryption.md)

## What is Encrypt.Config

In production environments, configuration files will more than likely contain sensitive information. Encrypt.Config is a tool designed to help protect JSON based configuration files by encrypting them. When used in combination with [Decrypt.Config](https://github.com/Supercide/Decrypt.Config) an ASP Core application will be able to use encrypted configuration files. Encrypt.Config does this by first Creating a public and private key on the machine where you want to store the encrypted configuration file. You then export the public key and use it to encrypt the configuration file. More information on how this works below 

# Getting Started 
Check out the [Docs](/docs) folder for instructions on how to use the application 

# Current day issues
Encryption comes with its own set of unique challenges. Encrypt config Aims to combat the following issues

## Key distribution 
#### how do we convey keys to those who need them to establish secure communication? 
Asymmetric keys solves this issue by separating the keys used for encryption and decryption. The encryption key can be made public but the decryption key must be kept private. Asymmetric encryption is done via RSA.

## Key management
#### how do we preserve key safety and make them available as needed? 
All windows machines from Windows XP and above have the ability to securly store keys in RSA containers. [See here](https://msdn.microsoft.com/library/9a179f38-8fb7-4442-964c-fb7b9f39f5b9) for more details.
 
## Trust
#### How do we ensure that the encrypted message recieved came from a trusted source? 
We can ensure this by creating two sets of public/private keys for each participent, one set of keys is for signing messages and the other for encrypting data. When Alice wants to send a message to Bob, she will have to make sure Bob has her public signing key and that she has Bob's public encryption key. When bob does have her public signing key she can then proceed to:
1. Create a symmetric session key used for data encryption
2. Encrypts her message with the newly created session key
3. Using the session key create a HMAC of the encrypted data
4. Using Bob's public encryption key, encrypt the session key
5. Using her private signing key, create a signatue of HMAC
6. Send the following to Bob
    - Encrypted message
    - Encrypted session key 
    - IV (initiation vector)
    - HMAC
    - session key signature

When Bob recieves the message he will:
1. Decrypt the session key
    - Using His private encryption key Bob decrypts the encrypted session key
2. Calculate the HMAC of the encrypted data. 
    - Using the decrypted session ky Bob recalculates the HMAC of the encrypted data. This ensures the encrypted message has not been corrupted during transport
3. Verify the signature
    - Using Alices public signing key Bob will verify the signature of the HMAC. This ensures that the message did indeed come from Alice
4. Decrypt the Message
    - using the session key with the IV Bob and proceed to decrypt the message 

# License

https://opensource.org/licenses/GPL-3.0