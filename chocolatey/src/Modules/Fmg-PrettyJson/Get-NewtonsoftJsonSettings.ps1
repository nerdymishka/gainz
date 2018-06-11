$fmgSerializerSettings = $null

# Load Newtonsoft
if([Type]::GetType("Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver") -eq $Null) {
    if($PSVersionTable.PSEdition -eq "Core") {
        [System.Reflection.Assembly]::LoadFile("$PSScriptRoot\bin\netstandard1.3\Newtonsoft.Json.dll")
    } else {
        [System.Reflection.Assembly]::LoadFile("$PSScriptRoot\bin\net45\Newtonsoft.Json.dll")
    }
}



function New-NewtonsoftJsonSettings() {
<#
    .SYNOPSIS
    Creates a new Settings object.

    .DESCRIPTION
    Creates a new object of Newtonsoft.Json.JsonSerializerSettings

    .EXAMPLE
    $settings = New-NewtonsoftJsonSettings

#>
    return New-Object Newtonsoft.Json.JsonSerializerSettings
}

function Set-NewtonsoftJsonSettings() {
<#
    .SYNOPSIS
    Sets the default global Newtonsoft Json Settings object.

    .DESCRIPTION
    If a settings object is not passed to ConvertTo-NewtonsoftJson,
    then the function attempts to get the global default settings object.

    By default, one is created with the camel casing converter enabled.

    The global object can be changed by calling this cmdlet:
    Set-NewtonsoftJsonSettings

    .PARAMETER Settings
    Mandatory. The settings object that will be used as the default 
    global settings object.


    .EXAMPLE
    PS C:\>$settings = New-NewtonsoftJsonSettings 
    PS C:\>$camelCaseResolver = "Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver" 
    PS C:\>$settings.ContractResolver = New-Object $camelCaseResolver 
    PS C:\>Set-NewtonsoftJsonSettings $settings
#>
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [Newtonsoft.Json.JsonSerializerSettings] $Settings 
    )

    $fmgSerializerSettings = $Settings
    Set-Variable -Name "fmgSerializerSettings" -Value $Settings -Scope Script
}

function Get-NewtonsoftJsonSettings() {
<#
    .SYNOPSIS
    Gets the default global Newtonsoft Json Settings object.

    .DESCRIPTION
    Gets the default global Newtonsoft Json Settings object.

    .EXAMPLE
    $settings = Get-NewtonsoftJsonSettings
#>  
   
    $fmgSerializerSettings = Get-Variable -Name "fmgSerializerSettings" -Scope Script
    $fmgSerializerSettings = $fmgSerializerSettings.Value
    if(!$fmgSerializerSettings) {
        
        $fmgSerializerSettings = New-NewtonsoftJsonSettings
        $camelCaseResolver = "Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver"
        $fmgSerializerSettings.ContractResolver = New-Object $camelCaseResolver
        Set-NewtonsoftJsonSettings -Settings $fmgSerializerSettings
    }

    return $fmgSerializerSettings;
}
