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

Encryption comes with its own set of unique challenges how do we manage keys? How do we distribute keys securely? How can we trust this message came from a trusted source? The techniques used in Encrypt.Config answers most of this common issues. For more information on how it works read below or dive right into the code here [Show me the code!!!](./docs).

## Key distribution, Management & Trust
If keys are stolen along with then encrypted data, then the attacker has the ability to decrypt all your data: Not an ideal position to be in. Depending on the type of encryption used the attacker will also be able to forge messages, also not ideal. Key management is the hardest issue to solve; Where do you store keys? How do you transport keys safely?

Encrypt.Config uses Hybrid encryption, which is a mixture of symmetric and asymmetric algorithms. Symmetric keys are more efficient at encrypting large amounts of data compared to asymmetric keys. This is because asymmetric keys include extra randomness in order to protect itself against brute force attacks. For example without this extra randomness an attacker using the public key could encrypt potential words until they find a match in the encrypted message. A side effect of the extra randomness is that encrypted messages end up being significantly larger then the original message and encryption becomes computationally more expensive the larger the message to encrypt. Symmetric keys break up data into individual blocks for encrypting. Depending on the type of cipher mode it may chain the blocks together feeding the last bits of the previous block into the next block so that if the 2 blocks containing the same bits of data don't encrypt to the same value. This method is faster because it does not need to expand the data being encrypted whereas asymmetric keys do. 

Symmetric keys in Encrypt.Config are used to encrypt data. However if an attacker was to get a hold of the symmetric key they could decrypt all encrypted messages. To prevent this from happening asymmetric keys are used. using the public key we encrypt the symmetric key to protect it from potential attackers. A signature is also calculated with the private key. The signature, encrypted symmetric key and encrypted data are then sent to Alice. Now if an attacker gets the encrypted symmetric key they would not be able to decrypt the message. They also wont be able to generate a valid signature from the public key so forged messages would fail verification checks. It goes without saying that storage of the private key has to be secure. One way to secure your private key would be to use RSA key container on windows [See here](https://msdn.microsoft.com/library/9a179f38-8fb7-4442-964c-fb7b9f39f5b9) for more information.
 
 ## Step by step example
  When Alice wants to send a message to Bob, she will have to make sure Bob has her public signing key and that she has Bob's public encryption key. When Bob does have her public signing key she can then proceed to:
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