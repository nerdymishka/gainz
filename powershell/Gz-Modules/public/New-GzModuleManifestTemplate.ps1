function New-GzModuleManifestTemplate() {
    Param(
        [String] $Destination,

        [Switch] $PassThru
    )



    $json =  @{
        "Author" = "$ENv:USERNAME"
        "Company" = "My Company"
        "Copyright" = "$([DateTime]::Now.Year) My Company"
        "Tags" = @("One", "Two")
        "IconUri" = ""
        "LicenseUri" = ""
        "ProjectUri" = ""
    } | ConvertTo-Json 

    if($PassThru.ToBool()) {
        return $Json 
    }

    if([string]::IsNullOrWhiteSpace($Destination)) {
        $Destination = "$HOME/.config/gz/gz-modulemanifest.json"
    }

    $directory = $Destination | Split-Path 

    if(!(Test-Path $directory)) {
        New-Item $directory -ItemType Directory | Write-Debug
    }

    $json | Out-File $Destination -Encoding "UTF8"
}