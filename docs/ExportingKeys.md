[Getting Started](../README.MD) > [Creating Keys](./CreatingKeys.md) > [Encrypting Files](./Encryption.md) > Exporting Keys
# Exporting Keys

>Warning!
>
>You will only be able to export keys if you have permission on the RSA key container. If you followed the walk through, only the application identity can export the keys. If you find you can't export keys due to lack of permissions contact your administrator, any other issues raise a issue on the github repository.

If there ever comes a time where you need to gain access to these keys you can use the following command:

`export -e <Path and file name to export key> -(pb|pv)<Key type> -c <Name of container storing keys>`

where `pb` is for public key and `pv` is for private 
>Warning!
>
>Exporting the private key is not recommended. If this key is compromised then all data encrypted with it's public counterpart can be decrypted

##### Example:

```
export -e public.key -pb -c helloWorldApi-encryption-keys
```

[Getting Started](../README.MD) > [Creating Keys](./CreatingKeys.md) > [Encrypting Files](./Encryption.md) > Exporting Keys