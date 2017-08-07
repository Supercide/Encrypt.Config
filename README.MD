## What is Encrypt.Config

In production environments, configuration files will more than likely contain sensitive information. Encrypt.Config is a tool designed to help protect JSON based configuration files by encrypting them. When used in combination with [Decrypt.Config](#TODO) an ASP Core application will be able to use encrypted configuration files, decrypting values on access. Encrypt.Config does this by first Creating a public and private key on the machine where you want to store the encrypted configuration file. You then export the public key and use it to encrypt the configuration file. For more information on how this works [click here](#TODO)

## How

![Diagram of how it works](./Diagram-How.jpg)

1. First Create the Private and public keys against the identity of the application whose configuration files need to be encrypted

2. Export the public key to a location where the encryption will take place (this may be your deployment server or build server, its typically the place where configuration file transformation happens)

3. After configuration files have been transformed run Encrypt.Config against the configuration file passing in the public key as an argument

4 deploy the configuration file and run the application, everything should work as if the configuration file wasn't encrypted

## Build status:

Latest: [![Build status](https://ci.appveyor.com/api/projects/status/gfh5l8uq94ws732x?svg=true)](https://ci.appveyor.com/project/jordan-Anderson/encrypt-config)

Master: [![Build status](https://ci.appveyor.com/api/projects/status/gfh5l8uq94ws732x/branch/master?svg=true)](https://github.com/Supercide/Encrypt.Config/tree/master)
