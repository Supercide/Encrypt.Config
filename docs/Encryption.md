[Getting Started](../README.MD) > Encryption
# Walk through
the following document will walk you through encrypting files. If you haven't already read how to create keys check it out [here](./CreatingKeys.md)

There are 3 steps Encrypt config goes through when encrypting files.

1. Create a session key. This key will be used to do the actual file encryption
2. Encrypt the file using the session key.
3. Encrypt the session key. It is vital to encrypt the session key before it is distributed as it is used for decrypting the file
4. Create a hmac hash of the encrypted data using the unencrypted session key. this ensures the encrypted data is still intact.
5. Sign the data with the machines private key
