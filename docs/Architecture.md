[Getting Started](../README.MD) > Architecture
# Architecture
- .NET Framework 4.7 runtime
- Windows XP and above
- NTFS partition 

>.NET Framework 4.7 runtime is only needed during the encryption phase

>The windows os constraint is due to how windows protects keys RSA keys, see [Link](https://msdn.microsoft.com/library/9a179f38-8fb7-4442-964c-fb7b9f39f5b9) for details. 

>The application makes use of NTFS ACL to control access to sensitive files used to store decryption keys as per recommendation from Microsoft. See [Link](https://msdn.microsoft.com/en-gb/library/windows/desktop/aa374872(v=vs.85).aspx) for details.

# Key distribution 
how do we convey keys to those who need them to establish secure communication? Asymmetric keys solves this issue by separating the keys used for encryption and decryption. The encryption key can be made public but the decryption key must be kept private. Asymmetric encryption is done via RSA.

# Key management
how do we preserve their safety and make them available as needed?Luckily windows provides us with a way to store keys securely in RSA containers. [See here](https://msdn.microsoft.com/library/9a179f38-8fb7-4442-964c-fb7b9f39f5b9) for more details.
 
