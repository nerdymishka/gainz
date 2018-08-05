function New-ProtectBlobKey() {
    Param(
        [System.Int32] $KeySize
    )

    if($KeySize -lt 1) {
        throw [ArgumentException] "KeySize must be greater than 0"
    }

    $bytes = New-Object 'byte[]' -ArgumentList ($KeySize / 8)
    $rng = [System.Security.Cryptography.RandomNumberGenerator]::Create();
    $rng.GetBytes($bytes)
    $rng.Dispose()

    return $bytes;
}

