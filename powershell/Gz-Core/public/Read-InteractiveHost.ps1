
function Read-InteractiveHost() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [String] $Prompt,
        [Switch] $AsSecureString,
        [string] $DefaultValue = $null
    )

    if(Test-NonInteractive) {
        if($AsSecureString.ToBool()) {
            if(![String]::IsNotNullOrWhitespace($DefaultValue)) {
                return ConvertTo-SecureString $DefaultValue -AsPlainText -Force 
            }
            return New-Object [System.Security.SecureString]
        }

        return $DefaultValue
    }

    return Read-Host -Prompt:$Prompt -AsSecureString:$AsSecureString
}