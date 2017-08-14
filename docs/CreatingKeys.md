[Getting Started](../README.MD) > Creating Keys > [Encrypting Files](./Encryption.md) > [Exporting Keys](./ExportingKeys.md)

# Are my keys safe
The main dangers that come with encryption is somebody getting a hold of your private keys. To help prevent this from happening Encrypt config stores all private keys in windows RSA key containers with additional access control rules to restrict access to only authorized users

>Key containers provide the most secure way to persist cryptographic keys and keep them secret from malicious third parties. Click [here](https://msdn.microsoft.com/library/9a179f38-8fb7-4442-964c-fb7b9f39f5b9) for more details.

>It is recommended that you only secure sensitive information using protected configuration on file systems formatted using NTFS, so that you can restrict access to encryption key information using ACLs.

# Walkthrough
In this example we will take the common scenario of a deployment server and a web application server.

## Step 1 - Creating the signing keys
The first step is to create a set of signing keys. These keys will ensure that the encrypted message came from a trusted source, in our instance we want to verify that encrypted files have come from our deployment server.

On your deployment server run the following command:

`create -u <Application NT Identity> -c <Windows RSA Container name> -e <Path to export signing key>`

Where the application nt identity is the identity of which your deployment tool runs under. Name the RSA container descriptive enough that you know this is the container for the signing keys. 

##### Example:
```
create -u deploymentServer/OctopusDeploy -c octopus-signing-key -e sig.key
```

This command will generate a 2048 bit public/private key stored securly in a RSA container. It also exports the public key needed for verification. Store this key somewhere where the it is accessible to the application

## Step 2 - Creating the encryption keys

Simmilar to Step one we are now going to create the keys needed for file encryption.

On the web application server run the following command

`create -u <Application NT Identity> -c <Windows RSA Container name> -e <Path to export encryption key>`

Where the application nt identity is the identity of which your web application. Name the RSA container descriptive enough that you know this is the container for the encrypting/decryping files.

##### Example:
```
create -u webserver/HelloWorldApi -c helloWorldApi-encryption-keys -e encryption.key
```

This will produce a public key needed for the file encryption. This key needs to be accessible by the deployment server to be able to encrypt files before deploying. 

>It should be noted that this key will only encrypt data for the application it was created for. If you need to encrypt data for any other application then you must repeate the process apart from creating the deployment siging key as that key is shared

[Getting Started](../README.MD) > Creating Keys > [Encrypting Files](./Encryption.md) > [Exporting Keys](./ExportingKeys.md)