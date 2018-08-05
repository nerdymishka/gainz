function Protect-String() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $True)]
        [String] $Value,

        [String] $Encoding = "UTF-8",

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

    $enc = [System.Text.Encoding]::GetEncoding($Encoding)
    $blob = $enc.GetBytes($Value)

    if($Password) {
        $result = Protect-Blob -Blob $blob -Password $Password -InsecureInfo $InsecureInfo
    } elseif($PrivateKey) {
        $result = Protect-Blob -Blob $blob -PrivateKey $PrivateKey -InsecureInfo $InsecureInfo
    } else {
        $result = Protect-Blob -Blob $blob -Key $Key `
          -AuthenticationKey $AuthenticationKey -InsecureInfo $InsecureInfo
    }

    return [Convert]::ToBase64String($result)
}
