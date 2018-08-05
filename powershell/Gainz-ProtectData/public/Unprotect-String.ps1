

function Unprotect-String() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [String] $EncryptedValue,

        [String] $Encoding = "UTF-8",

        [Parameter(ParameterSetName = "PrviateKey")]
        [Byte[]] $PrivateKey,

        [Parameter(ParameterSetName = "Password")]
        [SecureString] $Password,

        [Parameter(ParameterSetName = "Keys")]
        [Byte[]] $Key = $null,

        [Parameter(ParameterSetName = "Keys")]
        [Byte[]] $AuthenticationKey = $null,

        [Byte[]] $InsecureInfo = $null
    )

    $blob = [Convert]::FromBase64String($EncryptedValue)
    if($Password) {
        $result = Unprotect-Blob -EncryptedBlob $blob -Password $Password -InsecureInfo $InsecureInfo
    } elseif($PrivateKey) {
        $result = Unprotect-Blob -EncryptedBlob $blob -PrivateKey $PrivateKey -InsecureInfo $InsecureInfo
    } else {
        $result = Unprotect-Blob -EncryptedBlob $blob -Key $Key `
          -AuthenticationKey $AuthenticationKey -InsecureInfo $InsecureInfo
    }


    $enc = [System.Text.Encoding]::GetEncoding($Encoding)
    return $enc.GetString($result)
}