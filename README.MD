# Encrypt.Config
[![Build status](https://ci.appveyor.com/api/projects/status/gfh5l8uq94ws732x/branch/master?svg=true)](https://github.com/Supercide/Encrypt.Config/tree/master)

File encryption tool aimed at encrypting configuration files (Json/XML/text/ect..)

## Table of contents
- [Read Me](./README.MD)
- [Architecture](./docs/Architecture.md)
- Instruction Manual
    - [Creating Keys](./docs/CreatingKeys.md)
    - [Encrypting](./docs/Encryption.md)
    - [Exporting Keys](./docs/ExportingKeys.md)

## What is Encrypt.Config

In production environments, configuration files will more than likely contain sensitive information. Encrypt.Config is a tool designed to help protect configuration files by encrypting them. When used in combination with [Decrypt.Config](https://github.com/Supercide/Decrypt.Config) an ASP Core application will be able to use any encrypted files produced by Encrypt.Config. 

Encryption comes with its own set of unique challenges how do we manage keys? How do we distribute keys securely? How can we trust this message came from a trusted source?The techniques used in Encrypt.Config answers most of this common issues. For more information on how it works read below or dive right into the code here [Show me the code!!!](./docs).

## Key distribution 
#### how do we convey keys to those who need them to establish secure communication? 
Asymmetric algorithms solve this issue by separating keys used for encryption and decryption into Public and Private keys. The only thing you can do with a public key is encrypt data and verify signatures. Ifthis key was to get into the hands of somebody who the key was not intended for all they could do is encrypt data. The private key however is used for message decryption and signing data. It goes without saying that the storage and handling of the private key must be handled with care.

## Key management
#### how do we preserve key safety and make them available as needed? 
All windows machines from Windows XP and above have the ability to securely store keys in RSA containers. [See here](https://msdn.microsoft.com/library/9a179f38-8fb7-4442-964c-fb7b9f39f5b9) for more information.
 
## Trust
#### How do we ensure that the encrypted message received came from a trusted source? 
We can ensure this by creating two sets of public/private keys for each participant, one set of keys is for signing messages and the other for encrypting data. When Alice wants to send a message to Bob, she will have to make sure Bob has her public signing key and that she has Bob's public encryption key. When Bob does have her public signing key she can then proceed to:
1. Create a symmetric session key used for data encryption
2. Encrypts her message with the newly created session key
3. Using the session key create a HMAC of the encrypted data
4. Using Bob's public encryption key, encrypt the session key
5. Using her private signing key, create a signature of HMAC
6. Send the following to Bob
    - Encrypted message
    - Encrypted session key 
    - IV (initiation vector)
    - HMAC
    - session key signature

When Bob receives the data from point 6 he will:
1. Decrypt the session key
    - Using His private encryption key Bob decrypts the encrypted session key
2. Calculate the HMAC of the encrypted data. 
    - Using the decrypted session key Bob recalculates the HMAC of the encrypted data. This ensures the encrypted message has not been corrupted during transport
3. Verify the signature
    - Using Alice's public signing key Bob will verify the signature of the HMAC. This ensures that the message did indeed come from Alice
4. Decrypt the Message
    - using the session key with the IV Bob and proceed to decrypt the message

# What doesn't it solve?
For now Encrypt.Config does not manage storing of the public keys securely, we leave this up to you to figure out. This means if you store them with inadequate protection and an attacker gains physical access to your keys theoretically they would be able to forge messages between systems by replacing these keys.

##### Example:
Attacker gains access to Bob's pc, replacing Alice's public key with there own. Attacker creates a message, encrypts it with Alice's public key and signs it with there own signature key. Attacker also gains access to Alice's machine and swaps Bob's signature key with there key. Attacker can now send messages from Bobs machine to Alice's machine tricking Alice to believing the message came from Bob

You may be able to prevent this from happening through the use of DPAPI, NTFS ACL's or EFS but some thought will be require on storing public keys.

# License

https://opensource.org/licenses/GPL-3.0