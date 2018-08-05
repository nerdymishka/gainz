function ConvertTo-UnprotectedBytes() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [SecureString] $SecureString,

        [String] $Encoding = "UTF-8"
    )



    $enc = [System.Text.Encoding]::GetEncoding($Encoding)

    $bstr = [IntPtr]::Zero;
    $charArray = New-Object 'char[]' -ArgumentList $SecureString.Length

    try
    {

        $bstr = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($SecureString);
        [System.Runtime.InteropServices.Marshal]::Copy($bstr, $charArray, 0, $charArray.Length);

        $bytes = $enc.GetBytes($charArray);
        return $bytes
    }
    finally
    {
        [System.Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr);
    }
}
