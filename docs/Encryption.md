[Getting Started](../README.MD) > [Creating Keys](./CreatingKeys.md) > Encryption

## Encrypting files
Now that the hard part is done you can start encrypting files to your hearts content. To encrypt a file run the following command on the deployment server:

`encrypt -i <Path to apps public encryption key> -s <name of the signature container> -f <file to encrypt> -o <file to save encrypted file>`

##### Example:
```
encrypt -i encryption.key -s octopus-signing-key -f appsettings.json -o appsettings.encrypted
```

This will produce a encrypted file that can on that can only be decrypted by the application whos key was used. It will also produce a decryption key, don't worry this key alone cannot decrypt the message and there is nothing sensitive in it. You can read more about the purpose of this file [Here](../README.MD) 
> the decryption key we produce in this step is comprised of:
>  - Encrypted session key 
>  - IV (initiation vector)
>  - HMAC
>  - session key signature


The web application will be able to use its private key and the key we produced during encryption to decrypt the file. No other application will be able to decrypt this file.