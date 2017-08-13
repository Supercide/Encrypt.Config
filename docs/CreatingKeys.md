[Getting Started](../README.MD) > Creating Keys

# Walk through
the following document will walk you through creating keys for encrypting and decrypting files. In total there are 3 keys and a hash, each has there specific use cases:

- Private keys are used for decrypting session keys which in turn are used to decrypt files. Private keys should only be stored on the same machine where the application is running. 

- Public keys are used to encrypt session keys. These keys can be used from any machine whishing to encrypt a file.

- Symmetric keys are used to encrypt actual file contents. compared to Asymmetric keys symmetric keys can encrypt large file more efficiently.

- HMAC Hash ensures that the encrypted session key wasn't damaged or tamperer with in any way

# Are my keys safe
The main dangers that come with encryption is somebody getting a hold of your private keys. To help prevent this from happening Encrypt config stores all private keys in windows RSA key containers with additional access control rules to restrict access to only authorized users

>Key containers provide the most secure way to persist cryptographic keys and keep them secret from malicious third parties. Click [here](https://msdn.microsoft.com/library/9a179f38-8fb7-4442-964c-fb7b9f39f5b9) for more details.

>It is recommended that you only secure sensitive information using protected configuration on file systems formatted using NTFS, so that you can restrict access to encryption key information using ACLs.

# Commands & examples

### Create Public/Private keys 

#### command

`create -usr <Application NT Identity> -nme <Windows RSA Container name>`

#### Example

```
# create public private keys

create -usr desktop/webapplication -nme webapplication-config
```

#### Response

Creates a RSA container at `C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys` with limited access rules

### Create and export Public/Private keys

#### command

`create -usr <Application NT Identity> -nme <Windows RSA Container name> -exp <path of exported key> -pb`

#### Example

```
# create public/private keys and export public key

create -usr desktop/webapplication -nme webapplication-config -exp .\publicKey -pb
```

#### Response

Creates a RSA container at `C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys` with limited access rules

Creates a public key file at the specified location

