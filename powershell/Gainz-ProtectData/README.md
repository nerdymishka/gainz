# Gainz ProtecteData

Protect data will encrypt/decrypt data, generate a unique salt, and verify the
data integrity with a checkum.

```powershell
$privateKey =  Read-Host -AsSecureString
$privateKey = $privateKey | ConvertTo-UnprotectedBytes

# takes a string and returns a string
$encryptedDataAsString = Protect-String "special value" -PrivateKey $privateKey
$decryptedString = Unprotect-String $encryptedDataAsString -PrivateKey $privateKey

# takes a string and returns a string
$bytes = [System.IO.File]::ReadAllBytes("$Home/Desktop/test.txt")
$encryptedData = Protect-Blob $bytes -PrivateKey $privateKey
$decryptedData = Unprotect-Blob $encryptedData -PrivateKey $privateKey

[Array]::Clear($privateKey, 0, $privateKey.Length)
```