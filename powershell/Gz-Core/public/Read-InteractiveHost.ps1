
function Read-InteractiveHost() {
    Param(
        [String] $Prompt,
        [Switch] $AsSecureString,
        [string] $DefaultValue = $null
    )

    if(Test-ShellNonInteractive) {
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