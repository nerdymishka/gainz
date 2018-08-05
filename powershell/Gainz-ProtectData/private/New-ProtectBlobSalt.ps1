function New-ProtectBlobSalt() {
    Param(
        [System.Int32] $SaltSize
    )

    if($SaltSize -lt 1) {
        throw [ArgumentException] "KeySize must be greater than 0"
    }

    $bytes = New-Object 'byte[]' -ArgumentList ($SaltSize / 8)
    $rng = [System.Security.Cryptography.RandomNumberGenerator]::Create();
    $rng.GetBytes($bytes)
    $rng.Dispose()

    return $bytes;
}
