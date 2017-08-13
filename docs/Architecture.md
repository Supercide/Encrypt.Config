[Getting Started](../README.MD) > Architecture
# Architecture
- .NET Framework 4.7 runtime
- Windows XP and above
- NTFS partition 

>.NET Framework 4.7 runtime is only needed during the encryption phase

>The windows os constraint is due to how windows protects keys RSA keys, see [Link](https://msdn.microsoft.com/library/9a179f38-8fb7-4442-964c-fb7b9f39f5b9) for details. 

>The application makes use of NTFS ACL to controll access to sensitive files used to store decryption keys as per recommendation from Microsoft. See [Link](https://msdn.microsoft.com/en-gb/library/windows/desktop/aa374872(v=vs.85).aspx) for details.
 
