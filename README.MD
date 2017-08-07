## What is Encrypt.Config

In production environments, configuration files will more than likely contain sensitive information. Encrypt.Config is a tool designed to help protect JSON based configuration files by encrypting them. When used in combination with [Decrypt.Config](#TODO) an ASP Core application will be able to use encrypted configuration files, decrypting values on access. Encrypt.Config does this by first Creating a public and private key on the machine where you want to store the encrypted configuration file. You then export the public key and use it to encrypt the configuration file. For more information on how this works [click here](#TODO)

## Build status:

Latest: [![Build status](https://ci.appveyor.com/api/projects/status/gfh5l8uq94ws732x?svg=true)](https://ci.appveyor.com/project/jordan-Anderson/encrypt-config)

Master: [![Build status](https://ci.appveyor.com/api/projects/status/gfh5l8uq94ws732x/branch/master?svg=true)](https://github.com/Supercide/Encrypt.Config/tree/master)
