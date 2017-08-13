# Architecture
- .NET Framework 4.7 runtime
- Windows XP and above
- NTFS partition 

>.NET Framework 4.7 runtime is only needed during the encryption phase

>The windows os constraint is due to how windows protects keys RSA keys, see [Link]() for details. 

>The application makes use of NTFS ACL to controll access to sensitive files used to store decryption keys as per recommendation from Microsoft. See [Link]() for details.
 
