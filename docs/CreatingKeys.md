[Getting Started](../README.MD) > Creating Keys

# Are my keys safe
The main dangers that come with encryption is somebody getting a hold of your private keys. To help prevent this from happening Encrypt config stores all private keys in windows RSA key containers with additional access control rules to restrict access to only authorized users

>Key containers provide the most secure way to persist cryptographic keys and keep them secret from malicious third parties. Click [here](https://msdn.microsoft.com/library/9a179f38-8fb7-4442-964c-fb7b9f39f5b9) for more details.

>It is recommended that you only secure sensitive information using protected configuration on file systems formatted using NTFS, so that you can restrict access to encryption key information using ACLs.

# Walkthrough
In this example we will take the common scenario of a deployment server and a web application server.

## Step 1 - Creating the signing keys
The first step is to create a set of signing keys. These keys will ensure that the encrypted message came from a trusted source.

On your deployment server run the following command:

`create -usr <Application NT Identity> -nme <Windows RSA Container name> -exp <Path to export signing key>`

Where the application nt identity is the identity of which your deployment tool runs under. Name the RSA container descriptive enough that you know this is the container for the signing keys. 

Example:
```
create -usr desktop/deploymentServer -nme deployment-signing-keys -exp sig.key
```

This command will generate a 2048 bit public/private key stored securly in a RSA container. It also exports the public key needed for verification. Store this key in a protected place, somewhere where the web application server can get access.

Repeat this process on the web application server, this time when the public key has been exported place it somewhere protected where the deployment server has access.

## Step 2 - Creating the Deployment server encryption keys

Simmilar to Step one we are now going to create the keys needed for file encryption.

On the web application server run the following command

`create -usr <Application NT Identity> -nme <Windows RSA Container name> -exp <Path to export signing key>`

Where the application nt identity is the identity of which your deployment tool runs under. Name the RSA container descriptive enough that you know this is the container for the signing keys. 

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

