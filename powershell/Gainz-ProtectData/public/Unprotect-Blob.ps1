
function Unprotect-Blob() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [Byte[]] $EncryptedBlob,

        [Parameter(ParameterSetName = "PrivateKey")]
        [Byte[]] $PrivateKey,

        [Parameter(ParameterSetName = "Password")]
        [SecureString] $Password,

        [Parameter(ParameterSetName = "Keys")]
        [Byte[]] $Key,

        [Parameter(ParameterSetName = "Keys")]
        [Byte[]] $AuthenticationKey,

        [Byte[]] $InsecureInfo
    )

    $Headers = $InsecureInfo
    if(!$Headers) {
        $Headers = New-Object 'Byte[]' -ArgumentList 0
    }

    $options = Get-ProtectOptions

    $saltLength = 0
    $AuthenticationSaltLength = 0
    if($Password -or $PrivateKey) {
        if($Password) {
            $PrivateKey = ConvertTo-UnprotectedBytes $Password
        } 
    

        $saltLength = $AuthenticationSaltLength = ($options.SaltSize / 8)
        $salt = New-Object 'Byte[]' -ArgumentList $saltLength
        $AuthenticationSalt = New-Object 'Byte[]' -ArgumentList $saltLength

        [Array]::Copy($EncryptedBlob, $Headers.Length, $salt, 0, $salt.Length)
        [Array]::Copy($EncryptedBlob, $Headers.Length + $salt.Length, $AuthenticationSalt, 0, $AuthenticationSalt.Length)

        $generator = New-Object System.Security.Cryptography.Rfc2898DeriveBytes `
            -ArgumentList $PrivateKey, ($salt), ($options.Iterations)

        $Key = $generator.GetBytes($options.KeySize / 8)
        $generator.Dispose()

        $generator = New-Object System.Security.Cryptography.Rfc2898DeriveBytes `
            -ArgumentList $PrivateKey, ($AuthenticationSalt), ($options.Iterations)

        $AuthenticationKey = $generator.GetBytes($options.KeySize / 8)

        $generator.Dispose()
        [Array]::Clear($pwd, 0, $pwd.Length)
    }

    if(!$Key -or $Key.Length -eq 0) {
        Throw [ArgumentException] "Key must not be null or empty"
    }

    if(!$AuthenticationKey -or $AuthenticationKey.Length -eq 0) {
        Throw [ArgumentException] "AuthenticationKey must not be null or empty"
    }


    $hmac = [System.Security.Cryptography.HMACSHA256]::Create()
    $hmac.Key = $AuthenticationKey
    $hash = New-Object 'Byte[]' -ArgumentList ($hmac.HashSize / 8)

    $chunk = $EncryptedBlob.Length - $hash.Length
    $message = New-Object 'Byte[]' -ArgumentList ($chunk)

  
    [Array]::Copy($EncryptedBlob, 0, $message, 0, $chunk)
    
    $computedHash = $hmac.ComputeHash($message)
    $ivSize = $options.BlockSize / 8

    if($EncryptedBlob.Length -lt ($hash.Length + $Headers.Length + $ivSize)) {
        return $null;
    }

    [Array]::Copy($EncryptedBlob, $EncryptedBlob.Length - $hash.Length, $hash, 0, $hash.Length)

  
    $compareResult = 0;
    for($i = 0; $i -lt $hash.Length; $i++) {
        $compareResult = $compareResult -bor ($hash[$i] -bxor $computedHash[$i])
    }
    
    if($compareResult -ne 0) {
        return $null;
    }
   
    $aes = [System.Security.Cryptography.Aes]::Create()
    $aes.KeySize = $options.KeySize
    $aes.BlockSize = $options.BlockSize
    $aes.Mode = "CBC"
    $aes.Padding = "PKCS7"

    $iv = New-Object 'Byte[]' -ArgumentList $ivSize
    $headerLength = ($Headers.Length + $salt.Length + $AuthenticationSalt.Length)
    [Array]::Copy($EncryptedBlob, $headerLength, $iv, 0, $iv.Length)
    $headerLength += $iv.Length

    $decryptor = $aes.CreateDecryptor($key, $iv)
    $ms = New-Object System.IO.MemoryStream
    $cryptoStream = New-Object System.Security.Cryptography.CryptoStream($ms, $decryptor, "Write")
    $binaryWriter = New-Object System.IO.BinaryWriter($cryptoStream)
   

    try {
        $binaryWriter.Write($EncryptedBlob, $headerLength, $EncryptedBlob.Length - $headerLength - $hash.Length)
        $cryptoStream.FlushFinalBlock()
        
        $ms.Flush()
       
        return $ms.ToArray()
    } finally {
       $binaryWriter.Dispose()
       $cryptoStream.Dispose()
       $ms.Dispose()
       $decryptor.Dispose();
    }
}

