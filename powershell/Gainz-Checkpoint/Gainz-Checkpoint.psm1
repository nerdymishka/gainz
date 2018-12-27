
$gzCheckPointFiles =  @{
    "__default" = "$env:ALLUSERSPROFILE/nerdy-mishka"
    "__defaultUser" = "$($HOME)/.config/nerdy-mishka"
}

$gzCheckPointStores = @{
    "__default" = $null 
    "__defaultUser" = $null
}

if($null -eq (Get-Command Test-UserIsElevated -EA SilentlyContinue)) {
    function Test-UserIsElevated() {
        [CmdletBinding()]
        Param(
            
        )
    
        Process {
            switch([Environment]::OsVersion.Platform) {
                "Win32NT" {
                    $identity = [System.Security.Principal.WindowsIdentity]::GetCurrent()
                    $admin = [System.Security.Principal.WindowsBuiltInRole]::Administrator
                    return ([System.Security.Principal.WindowsPrincipal]$identity).IsInRole($admin)
                }
                "Unix" {
                    $content = id -u
                    if($content -eq "0") {
                        return $true;
                    } 
        
                    return $false;
                }
                Default {
                    $plat = [Environment]::OsVersion.Platform
                    Write-Warning "$plat Not Supported"
                    return $false
                }
            }
        }
    }
}

function Add-GzCheckPointStore() {
    Param(
        [String] $Name,
        [String] $Path 
    )
}

function Read-GzCheckPointStore() {
    [CmdletBinding()]
    Param(
        [String] $Name,
        [Switch] $Force
    )

    DynamicParam {
        $nameAttribute = New-Object System.Management.Automation.ParameterAttribute
        $nameAttribute.Position = 0
        $nameAttribute;
       
        $nameAttribute.HelpMessage = "This product is only available for customers 21 years of age and older. Please enter your age:"

        $keys = @()
        foreach($k in $gzCheckPointFiles.Keys)
        {
            $keys += $k.ToString();
        }
        $nameValidateAttribute = New-Object System.Management.Automation.ValidateSetAttribute($keys)
        $attributeCollection = new-object System.Collections.ObjectModel.Collection[System.Attribute]

     
        $attributeCollection.Add($nameAttribute)
        $attributeCollection.Add($nameValidateAttribute);
  
        $nameParam = New-Object System.Management.Automation.RuntimeDefinedParameter('Name', [String], $attributeCollection)
        
        if(Test-UserIsElevated)
        {
            $nameParam.Value = "__default"
        }
        else
        {
            $nameParam.Value = "__defaultUser"
        }
        
        
    
        $paramDictionary = New-Object System.Management.Automation.RuntimeDefinedParameterDictionary
        $paramDictionary.Add('Name', $nameParam)
        return $paramDictionary
    }

    PROCESS {
        $name = $PSBoundParameters['Name'];

        if([string]::IsNullOrWhiteSpace($name)) {
            throw [System.ArgumentNullException] '-Name'
        }

        if(!$gzCheckPointFiles.ContainsKey($name)) {
            throw [System.ArgumentException] "-Name $name for CheckPoint store is invalid"
        }

        $config = $gzCheckPointStores[$name];

        if($config) {
            return $config;
        }

        $path = $gzCheckPointFiles[$name]


        if(!(Test-Path $path))
        {
            $content = @{ "__gz" = $true  }
            $config = $content;
            Write-GzCheckPointConfig -Name $name -Data $config 
        }
       
    
        $content = Get-Content $cfg -Raw | ConvertFrom-Json
    
        $data = @{}
        if($content) {
            $content | Get-Member -MemberType NoteProperty | Foreach { $data[$_.Name] = $_.Value }
            $gzCheckPointStores[$Name] = $data;
        }
      
        return $data;
    }
}

function Write-GzCheckPointStore() {
    Param(
       
        [Parameter(Position = 1)]
        [Hashtable] $Data
    )

    DynamicParam {
        $nameAttribute = New-Object System.Management.Automation.ParameterAttribute
        $nameAttribute.Position = 0
        $nameAttribute;
       
        $nameAttribute.HelpMessage = "This product is only available for customers 21 years of age and older. Please enter your age:"

        $keys = @()
        foreach($k in $gzCheckPointFiles.Keys)
        {
            $keys += $k.ToString();
        }
        $nameValidateAttribute = New-Object System.Management.Automation.ValidateSetAttribute($keys)
        $attributeCollection = new-object System.Collections.ObjectModel.Collection[System.Attribute]

     
        $attributeCollection.Add($nameAttribute)
        $attributeCollection.Add($nameValidateAttribute);
  
        $nameParam = New-Object System.Management.Automation.RuntimeDefinedParameter('Name', [String], $attributeCollection)
        
        if(Test-UserIsElevated)
        {
            $nameParam.Value = "__default"
        }
        else
        {
            $nameParam.Value = "__defaultUser"
        }
        
        
    
        $paramDictionary = New-Object System.Management.Automation.RuntimeDefinedParameterDictionary
        $paramDictionary.Add('Name', $nameParam)
        return $paramDictionary
    }

    PROCESS {

        $name = $PSBoundParameters['Name'];

        if([string]::IsNullOrWhiteSpace($name)) {
            throw [System.ArgumentNullException] '-Name'
        }

        $path = $gzCheckPointFiles[$Name];

        if(!(Test-Path $path)) {
            $dir = Split-Path $path  
            mkdir $dir -Force
        }
    
        $c =  $data | ConvertTo-Json -Depth 10 
        $c | Out-File -FilePath $path -Encoding "UTF8" -Force

    }
}

# Without checkpoint funactionality, the script will re-run everything upon rebooting.
# This creates the potential for an infinite loop and it definitely slows down the install process.
function Test-Checkpoint() {
    [CmdletBinding()]
    Param(
        [Parameter(Position = 0)]
        [String] $CheckPoint
    )

    DynamicParam {
        $nameAttribute = New-Object System.Management.Automation.ParameterAttribute
        $nameAttribute.Position = 1
        $nameAttribute;
       
        $nameAttribute.HelpMessage = "This product is only available for customers 21 years of age and older. Please enter your age:"

        $keys = @()
        foreach($k in $gzCheckPointFiles.Keys)
        {
            $keys += $k.ToString();
        }
        $nameValidateAttribute = New-Object System.Management.Automation.ValidateSetAttribute($keys)
        $attributeCollection = new-object System.Collections.ObjectModel.Collection[System.Attribute]

     
        $attributeCollection.Add($nameAttribute)
        $attributeCollection.Add($nameValidateAttribute);
  
        $nameParam = New-Object System.Management.Automation.RuntimeDefinedParameter('StoreName', [String], $attributeCollection)
        
        if(Test-UserIsElevated)
        {
            $nameParam.Value = "__default"
        }
        else
        {
            $nameParam.Value = "__defaultUser"
        }
        
        
    
        $paramDictionary = New-Object System.Management.Automation.RuntimeDefinedParameterDictionary
        $paramDictionary.Add('StoreName', $nameParam)
        return $paramDictionary
    }

    PROCESS {
        $Name = $PSBoundParameters['StoreName']
        if([string]::IsNullOrWhiteSpace($NAme)) {
            if(Test-UserIsElevated) {
                $Name = "__default"
            } else {
                $Name = "__defaultUser"
            }
        }
        Write-Debug "Enter: Test-CheckPoint $CheckPoint"
        $data = Read-SolovisInstallConfig -Name $Name 
       
       
        if($data.ContainsKey($CheckPoint)) {
            Write-Debug "Contains $Checkpoint"
            return $true;
        }
    
        return $false;
    }
   
}

function Save-CheckPoint() {
    [CmdletBinding()]
    Param(
        [Parameter(Position = 0)]
        [String] $CheckPoint,


        [String] $StoreName,

        [Parameter(Position = 1)]
        [String] $ArgumentList
    )


    DynamicParam {
        $nameAttribute = New-Object System.Management.Automation.ParameterAttribute
        $nameAttribute.Position = 1
        $nameAttribute;
       
        $nameAttribute.HelpMessage = "This product is only available for customers 21 years of age and older. Please enter your age:"

        $keys = @()
        foreach($k in $gzCheckPointFiles.Keys)
        {
            $keys += $k.ToString();
        }
        $nameValidateAttribute = New-Object System.Management.Automation.ValidateSetAttribute($keys)
        $attributeCollection = new-object System.Collections.ObjectModel.Collection[System.Attribute]

     
        $attributeCollection.Add($nameAttribute)
        $attributeCollection.Add($nameValidateAttribute);
  
        $nameParam = New-Object System.Management.Automation.RuntimeDefinedParameter('StoreName', [String], $attributeCollection)
        
        if(Test-UserIsElevated)
        {
            $nameParam.Value = "__default"
        }
        else
        {
            $nameParam.Value = "__defaultUser"
        }
        
        
    
        $paramDictionary = New-Object System.Management.Automation.RuntimeDefinedParameterDictionary
        $paramDictionary.Add('StoreName', $nameParam)
        return $paramDictionary
    }

    PROCESS {
      
        $Name = $PSBoundParameters['StoreName']
        if([string]::IsNullOrWhiteSpace($NAme)) {
            if(Test-UserIsElevated) {
                $Name = "__default"
            } else {
                $Name = "__defaultUser"
            }
        }
        Write-Debug "Enter: Save-CheckPoint $CheckPoint"

        $data = Read-SolovisInstallConfig -Name $Name 
        $data.Add($CheckPoint, $ArgumentList)
        Write-SolovisInstallConfig -Name $Name -Data $data 
    }
}


Export-ModuleMember -Function @(
    'Save-CheckPoint',
    'Test-Checkpoint',
    'Install-ChocolateyPkg',
    'New-Password'
)