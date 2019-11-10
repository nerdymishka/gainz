
$gzCheckPointFiles =  @{
    "__default" = "$Env:ALLUSERSPROFILE/gz"
    "__defaultUser" = "$($HOME)/.config/gz"
}

if([Environment]::OSVersion.Platform -ne [System.PlatformID]::Win32NT) {
    $gzCheckPointFiles["__default"] = "/var/gz"
}


$gzCheckPointStores = @{
    "__default" = $null 
    "__defaultUser" = $null
}



function Add-CheckPointStore() {
    Param(
        [String] $Name,
        [String] $Path 
    )
    
    if($gzCheckPointStores.ContainsKey($Name)) {
        if($Path -ne $gzCheckPointStores[$Name]) {
            Write-Warning "Checkpoint store already exists for $Name"
        }
        return;
    }

    $gzCheckPointStores[$Name] = @{ "__gz" = $true  }
    $gzCheckPointFiles[$Name] = $Path 
    if(Test-Path $Path) {
        return;
    }
    $dir = Split-Path $Path  
    if(!(Test-Path $dir)) {
        New-Item -ItemType Directory $dir -Force
    }

    $c =  $gzCheckPointStores[$Name] | ConvertTo-Json -Depth 10 
    $c | Out-File -FilePath $path -Encoding "UTF8" -Force
}

function Read-CheckPointStore() {
    [CmdletBinding()]
    Param(
        [Switch] $Force
    )

    DynamicParam {
        $nameAttribute = New-Object System.Management.Automation.ParameterAttribute
        $nameAttribute.Position = 0
       

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
        
        if(Test-UserIsAdministrator)
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

        if($name -eq "__defaultUser") {
            $path = "$($HOME)/.config/gz"
        } else {
            $path = $gzCheckPointFiles[$name]
        }

       


        if(!(Test-Path $path))
        {
            $content = @{ "__gz" = $true  }
            $config = $content;
            Write-CheckPointConfig -Name $name -Data $config 
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

function Write-CheckPointStore() {
    Param(
       
        [Parameter(Position = 1)]
        [Hashtable] $Data
    )

    DynamicParam {
        $nameAttribute = New-Object System.Management.Automation.ParameterAttribute
        $nameAttribute.Position = 0
       

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
        
        if(Test-UserIsAdministrator)
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

        if($Name -eq "__defaultUser") {
            $path = "$($HOME)/.config/gz"
        } else {
            $path = $gzCheckPointFiles[$Name];
        }

       

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
        [String] $Name
    )

    DynamicParam {
        $nameAttribute = New-Object System.Management.Automation.ParameterAttribute
        $nameAttribute.Position = 1
       
        

        $keys = @()
        foreach($k in $gzCheckPointFiles.Keys)
        {
            $keys += $k.ToString();
        }
        $nameValidateAttribute = New-Object System.Management.Automation.ValidateSetAttribute($keys)
        $attributeCollection = new-object System.Collections.ObjectModel.Collection[System.Attribute]

     
        $attributeCollection.Add($nameAttribute)
        $attributeCollection.Add($nameValidateAttribute);
  
        $nameParam = New-Object System.Management.Automation.RuntimeDefinedParameter('Store', [String], $attributeCollection)
        
        if(Test-UserIsAdministrator)
        {
            $nameParam.Value = "__default"
        }
        else
        {
            $nameParam.Value = "__defaultUser"
        }
        
        
    
        $paramDictionary = New-Object System.Management.Automation.RuntimeDefinedParameterDictionary
        $paramDictionary.Add('Store', $nameParam)
        return $paramDictionary
    }

    PROCESS {
        $Store = $PSBoundParameters['Store']
        if([string]::IsNullOrWhiteSpace($Store)) {
            if(Test-UserIsAdministrator) {
                $Store = "__default"
            } else {
                $Store = "__defaultUser"
            }
        }
        
        $data = Read-CheckPointStore -Name $Store 
       
       
        if($data.ContainsKey($Name)) {
            Write-Debug "Checkpoint $Name exists"
            return $true;
        }
    
        return $false;
    }
   
}

function Save-CheckPoint() {
    [CmdletBinding()]
    Param(
        [Parameter(Position = 0)]
        [String] $Name,

        [Parameter(Position = 1)]
        [System.Object] $Data
    )


    DynamicParam {
        $nameAttribute = New-Object System.Management.Automation.ParameterAttribute
        $nameAttribute.Position = 2
        
       
        $keys = @()
        foreach($k in $gzCheckPointFiles.Keys)
        {
            $keys += $k.ToString();
        }
        $nameValidateAttribute = New-Object System.Management.Automation.ValidateSetAttribute($keys)
        $attributeCollection = new-object System.Collections.ObjectModel.Collection[System.Attribute]

     
        $attributeCollection.Add($nameAttribute)
        $attributeCollection.Add($nameValidateAttribute);
  
        $nameParam = New-Object System.Management.Automation.RuntimeDefinedParameter('Store', [String], $attributeCollection)
        
        if(Test-UserIsAdministrator)
        {
            $nameParam.Value = "__default"
        }
        else
        {
            $nameParam.Value = "__defaultUser"
        }
        
        
    
        $paramDictionary = New-Object System.Management.Automation.RuntimeDefinedParameterDictionary
        $paramDictionary.Add('Store', $nameParam)
        return $paramDictionary
    }

    PROCESS {
      
        $StoreName = $PSBoundParameters['Store']
        if([string]::IsNullOrWhiteSpace($StoreName)) {
            if(Test-UserIsAdministrator) {
                $StoreName = "__default"
            } else {
                $StoreName = "__defaultUser"
            }
        }
        Write-Debug "Enter: Save-CheckPoint $CheckPoint"

        if($null -eq $Data) {
            $Data = $true
        }

        $store = Read-CheckPointStore -Name $StoreName 
        $store.Add($Name, $Data)
        Write-CheckPointStore -Name $StoreName -Data $store 
    }
}


Export-ModuleMember -Function @(
    'Save-CheckPoint',
    'Test-Checkpoint',
    'Add-CheckPointStore'
)