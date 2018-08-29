function New-KeePassEntry() {
    Param(
        [Parameter(ValueFromPipeline = $true, Mandatory = $true)]
        [NerdyMishka.KeePass.IKeePassPackage] $Package,
        
        [Parameter(Position = 0, Mandatory = $true)]
        [String] $Path,

        [SecureString] $Password,

        [Byte[]] $PasswordAsBytes,
 
        [String] $UserName,

        [String] $Uri,

        [String] $Notes,

        [String[]] $Tags,

        [Switch] $Force 
    )

    if($Password) {
        $PasswordAsBytes = $Password | ConvertTo-UnprotectedBytes 
    }

    if([string]::IsNullOrWhiteSpace($UserName)) {
        $UserName = $null;
    }

    if([string]::IsNullOrWhiteSpace($Uri)) {
        $Uri = $null;
    }

    if([string]::IsNullOrWhiteSpace($Notes)) {
        $Notes = $null;
    }

    $entry =  $Package.CreateEntry($Path, $PasswordAsBytes, $UserName, $Uri, $Notes, $Tags, $Force.ToBool())

    if($PasswordAsBytes) {
        [Array]::Clear($PasswordAsBytes, 0, $PasswordAsBytes.Length);
    }

    return $entry;
}