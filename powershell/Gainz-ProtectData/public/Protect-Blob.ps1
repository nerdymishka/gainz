

function Protect-Blob() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true )]
        [Byte[]] $Blob,

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

    $aes = [System.Security.Cryptography.Aes]::Create()
    $options = Get-ProtectOptions
    $aes.KeySize = $options.KeySize
    $aes.BlockSize = $options.BlockSize
    $aes.Mode = "CBC"
    $aes.Padding = "PKCS7"

    $Headers = $InsecureInfo
    if(!$Headers) {
        $Headers = New-Object 'Byte[]' -ArgumentList 0
    }
    $headerSize = ($options.SaltSize / 8) * 2
    $headerSize = $headerSize + $Headers.Length
    $header = New-Object 'byte[]' -ArgumentList $headerSize
    [Array]::Copy($headers, $header, $headers.Length)
    $headerIndex = $Headers.Length

    $aes.GenerateIV();
    $iv = $aes.IV
    if($Password -or $PrivateKey) {
        if($Password) {
            $PrivateKey = $Password  | ConvertTo-UnprotectedBytes 
        }
        
        $salt = New-ProtectBlobSalt -SaltSize $options.SaltSize
       
        $generator = New-Object System.Security.Cryptography.Rfc2898DeriveBytes `
            -ArgumentList $PrivateKey, $salt, ($options.Iterations)
        $salt = $generator.Salt
      
        $Key = $generator.GetBytes($options.KeySize / 8)

        [Array]::Copy($salt, 0, $header, $headerIndex, $salt.Length)
        $headerIndex += $salt.Length
        $generator.Dispose()

        $salt = New-ProtectBlobSalt -SaltSize $options.SaltSize
        $generator = New-Object System.Security.Cryptography.Rfc2898DeriveBytes `
            -ArgumentList $PrivateKey, $salt, ($options.Iterations)
        $salt = $generator.Salt

        $AuthenticationKey = $generator.GetBytes($options.KeySize / 8)

        [Array]::Copy($salt, 0, $header, $headerIndex, $salt.Length)
        $headerIndex += $salt.Length
        $generator.Dispose()

        [Array]::Clear($pwd, 0, $pwd.Length)
    }

    if(!$Key -or $Key.Length -eq 0) {
        Throw [ArgumentException] "Key must not be null or empty"
    }

    if(!$AuthenticationKey -or $AuthenticationKey.Length -eq 0) {
        Throw [ArgumentException] "AuthenticationKey must not be null or empty"
    }

    $encryptor = $aes.CreateEncryptor($key, $iv)
    $ms = New-Object System.IO.MemoryStream
    $cryptoStream = New-Object System.Security.Cryptography.CryptoStream($ms, $encryptor, "Write")
    $binaryWriter = New-Object System.IO.BinaryWriter($cryptoStream)

    $binaryWriter.Write($Blob)
    $binaryWriter.Flush()
    $cryptoStream.Flush()
    $cryptoStream.FlushFinalBlock()
    $ms.Flush()
    $encryptedBlob = $ms.ToArray()

    $binaryWriter.Dispose()
    $cryptoStream.Dispose()
    $ms.Dispose()

    $hmac = New-Object System.Security.Cryptography.HMACSHA256
    $hmac.Key = $AuthenticationKey
    $ms = New-Object System.IO.MemoryStream
    $binaryWriter = New-Object System.IO.BinaryWriter($ms)

    try {
    
        $binaryWriter.Write($Header);
        $binaryWriter.Write($iv);

        $binaryWriter.Write($encryptedBlob);
        $binaryWriter.Flush()
        $ms.Flush()
        $data = $ms.ToArray()
        $hash = $hmac.ComputeHash($data)
        $binaryWriter.Write($hash);

        $binaryWriter.Flush()
        $ms.Flush()

        return $ms.ToArray()
    } finally {
        $binaryWriter.Dispose()
        $ms.Dispose()
        $hmac.Dispose()
    }
}
