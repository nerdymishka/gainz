
function ConvertTo-NewtonsoftJson() {
<#
    .SYNOPSIS
    Converts the PsObject, Array, or Hashtable into a json string.

    .DESCRIPTION
    An alternate ConvertTo-Json method that outputs readable json unlike
    the native version for Powershell 5 and below. 

    .PARAMETER InputObject
    The Array, PsObject, or Hashtable object that should be serialized to json

    .PARAMETER Settings
    (Optional) The Newtonsoft.Json.JsonSerializerSettings object that will be used
    to serialized the Input Object.

    .EXAMPLE
    $jsonText = @("One", "Two") | ConvertTo-NewtonsoftJson
    
    .EXAMPLE
    PS C:\>$settings = New-NewtonsoftJsonSettings 
    PS C:\>$camelCaseResolver = "Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver" 
    PS C:\>$settings.ContractResolver = New-Object $camelCaseResolver
    PS C:\>$jsonText = @("One", "Two") | ConvertTo-NewtonsoftJson -Settings $settings
#>

    Param(
        [Parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $True )]
        [Object] $InputObject,
        [Parameter(Position = 1)]
        [Newtonsoft.Json.JsonSerializerSettings] $Settings 
    )

    if($Settings -eq $null) {
        $Settings = Get-NewtonsoftJsonSettings
    }

    $obj = Write-ObjectToNewtonsoftFriendlyValue -InputObject $InputObject

    return [Newtonsoft.Json.JsonConvert]::SerializeObject($obj, `
             [Newtonsoft.Json.Formatting]::Indented, `
             $Settings)
}

# Internal
function Write-ObjectToNewtonsoftFriendlyValue() {
    Param(
        [Parameter(Position  = 0)]
        [Object] $InputObject 
    )

    if($InputObject -eq $null) {
        return $null;
    }

    if($InputObject -is [Array]) {
        $result =  @()
        foreach($item in $InputObject) {
            $result += Write-ObjectToNewtonsoftFriendlyValue -InputObject $item 
        }
        return $result;
    } 

    if($InputObject -is [hashtable]) {
       
        $dictionary = New-Object "System.Collections.Generic.Dictionary[[string],[Object]]"
        foreach($key in $InputObject.Keys) {
            $value = Write-ObjectToNewtonsoftFriendlyValue -InputObject $InputObject[$key] 
            $dictionary.Add($key, $value)
        }
        return $dictionary
    }

    if($InputObject -is [psobject]) {
       
        $dictionary = New-Object "System.Collections.Generic.Dictionary[[string],[Object]]"
         $InputObject | Get-Member -MemberType NoteProperty | Foreach-Object {
            $name = $_.Name 
            $value = Write-ObjectToNewtonsoftFriendlyValue  -InputObject ($InputObject.$Name)
            $dictionary.Add($name, $value)
         }

         return $dictionary;
    }

    return $InputObject;
}