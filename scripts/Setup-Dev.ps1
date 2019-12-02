
<#
    Temporary Script
    The majorit of code already exists in other modules, but the modules have not been 
    tested and pushed to PowerShell Gallery yet. 
#>



$gzConfigDir = "$HOME/.config/gz"
$setupName = "gainz"

if(!(Test-Path $gzConfigDir)) {
    New-Item $gzConfigDir -ItemType Directory  
}

$checkPoint = @{};

if(Test-Path "$gzConfigDir/$setupName.setup.ckeckpoint.json") {
    
    $checkPoint = Get-Content -Raw "$gzConfigDir/$setupName.setup.checkpoint.json" | ConvertFrom-Json 
    $checkPoint = $checkPoint | ConvertTo-Hashtable
}


function Add-WindowsAutoRunScript() {
    Param(
        [Parameter(Position = 0)]
        [Alias("Name")]
        [String] $RunKeyName = "GainzReboot",

        [Parameter(Position = 1, ValueFromPipeline = $true)]
        [String] $Script,

        [Switch] $Force 
    )

    if(!(Test-Path $Script)) {
        throw "FileNotFound $Script";
    }

    $runPath ="HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\run"
 
    #PowerShell PATH = C:\WINDOWS\system32\WindowsPowerShell\v1.0\powershell.exe
    $powershellExe = (Join-Path $env:windir "system32\WindowsPowerShell\v1.0\powershell.exe")
    $value = "$PowershellExe -ExecutionPolicy Bypass -NonInteractive -File `"$Script`" " 

    $testKey = Get-ItemProperty -Path $runPath -Name $RunKeyName -EA SilentlyContinue
    if($testKey) {
        if(!$Force.ToBool()) {
            throw "AutoStartScript Key Exists $RunKeyName"
        }

        Remove-ItemProperty -Path $runPath -Name $RunKeyName | Write-Debug
    }

    New-ItemProperty -Path $runPath -Name $RunKeyName -Value $value | Write-Debug
}

function Remove-WindowsAutoRunScript() {
    Param(
        [Parameter(Position = 0)]
        [Alias("Name")]
        [String] $RunKeyName = "GainzReboot"
    )

    $runPath ="HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\run"
    $testKey = Get-ItemProperty -Path $runPath -Name $RunKeyName -EA SilentlyContinue
    if($testKey) {
        Remove-ItemProperty -Path $runPath -Name $RunKeyName | Write-Debug
    }
}

function Enable-WindowsAutoLogon() {
    Param(
        [Parameter(Mandatory = $true)]
        [PsCredential] $Credential 
    )

    $autoLogonPath = 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon'

    New-ItemProperty -Path $autoLogonPath `
        -Name AutoAdminLogon `
        -Value 1 `
        -Force

    New-ItemProperty -Path $autoLogonPath `
        -Name DefaultUserName `
        -Value $Credential.UserName `
        -Force 

    New-ItemProperty -Path $autoLogonPath `
        -Name DefaultPassword `
        -Value ($Credential.GetNetworkCredential().Password) `
        -Force
}

function Disable-WindowsAutoLogon() {
    Param(
        
    )

    $autoLogonPath = 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon'


    New-ItemProperty -Path $autoLogonPath `
        -Name AutoAdminLogon `
        -Value 0 `
        -Force

    $userName = Get-ItemProperty -Path $autoLogonPath `
        -Name DefaultUserName `
        -ErrorAction SilentlyContinue 

    if($userName) {
        Remove-ItemProperty -Path $autoLogonPath `
            -Name DefaultUserName `
            -Force 
    }

    $password =  Get-ItemProperty -Path $autoLogonPath `
        -Name DefaultPassword `
        -ErrorAction SilentlyContinue

    if($password) {
        Remove-ItemProperty -Path $autoLogonPath `
            -Name DefaultPassword `
            -Force 
    }
}

function Invoke-WindowsReboot() {
    Param(
        [Parameter(Position = 0)]
        [Alias("Name")]
        [String] $RunKeyName = "GainzReboot",

        [Switch] $AutoRun,

        [Switch] $Force 
    )

    if($AutoRun.ToBool()) 
    {
        $Script = $MyInvocation.PSCommandPath

        Add-AutoRunScript -Script $Script -Force 
    }

    Restart-Computer -Force
}

function Invoke-WindowsUpdateScript() {
    Param(
        [Parameter(Position = 0)]
        [Uri] $Uri,

        [Parameter(ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [PsCredential] $Credential,

        [String] $RunName = "SetupScript",

        [Switch] $RunOnReboot
    )

    if($Credential) {
        Enable-WindowsAutoLogin -Credential $Credential
    }
    $dir = "$($Env:ALLUSERSPROFILE)/gainz/updates"

    if(!$Uri.IsFile) {
        $localPath = $Uri.LocalPath;
        if($localPath) {
            $fileName = Split-Path $Uri.LocalPath -Leaf 
        } else {
            $fileName = [Guid]::NewGuid() + ".ps1"
        }

        if(!(Test-Path $dir)) {
            New-Item $dir -ItemType Directory -Force | Write-Debug
        }
        
        $script = "$dir/$fileName"
        Invoke-WebRequest -Uri $url -UseBasicParsing -OutFile $script 
    } else {
        $script = $Uri.LocalPath
    }



    if($Credential -or $RunOnReboot.ToBool()) {
        $wrapper = @"
`$ENV:AUTORUN = '1'
& `"$Script`"
"@

        if(!(Test-Path $dir)) {
            New-Item $dir -ItemType Directory -Force | Write-Debug
        }

        $updateScript = "$dir/update-script.ps1"
        $wrapper | Out-File $updateScript -Encoding "UTF8";
       

        Add-WindowsAutoRunScript -Script $updateScript -Name $RunName -Force 
        $pref = $ErrorActionPreference
        $ErrorActionPreference = "Stop"
        try {
            $Env:AUTORUN = '1'
            & $Script 
        } catch {
            $ex = $_.Exception 
            $now = [DateTime]::UtcNow.ToString()
            $msg += "[error]:[$now]"
            $msg += " " + $ex.Message 
            $msg += "`n" + $ex.StackTrace 
            $msg >> "$dir/error"

            # stop rebooting if there is an error
            Remove-WindowsAutoRunScript -Name $RunName
            Disable-WindowsAutoLogin

            throw $ex
        } finally {
            $ErrorActionPreference = $pref  
        }
      

        if(!(Test-WindowsPendingReboot)) {
            Remove-WindowsAutoRunScript -Name $RunName
            Disable-WindowsAutoLogin
        } else {
            Invoke-WindowsReboot
        }

        return 
    }



    $pref = $ErrorActionPreference
    $ErrorActionPreference = "Stop"
    try {
        $Env:AUTORUN = '1'
        & $Script 
    } catch {
        $ex = $_.Exception 
        $now = [DateTime]::UtcNow.ToString()
        $msg += "[error]:[$now]"
        $msg += " " + $ex.Message 
        $msg += "`n" + $ex.StackTrace 
        $msg >> "$dir/error"

       

        throw $ex
    } finally {
        $ErrorActionPreference = $pref  
    }
}

function Test-WindowsPendingReboot
{
    [CmdletBinding()]
    param(
        [Parameter(Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Alias("CN", "Computer")]
        [String[]]
        $ComputerName = $env:COMPUTERNAME,

        [Parameter()]
        [Switch] $Minimal,

        [Parameter()]
        [System.Management.Automation.PSCredential]
        [System.Management.Automation.CredentialAttribute()]
        $Credential,

        [Parameter()]
        [Switch]
        $Detailed,

        [Parameter()]
        [Switch]
        $SkipConfigurationManagerClientCheck,

        [Parameter()]
        [Switch]
        $SkipPendingFileRenameOperationsCheck
    )

    process
    {

        $set = @()
        foreach ($computer in $ComputerName)
        {
            try
            {
                $invokeWmiMethodParameters = @{
                    Namespace    = 'root/default'
                    Class        = 'StdRegProv'
                    Name         = 'EnumKey'
                    ComputerName = $computer
                    ErrorAction  = 'Stop'
                }

                $hklm = [UInt32] "0x80000002"

                if ($PSBoundParameters.ContainsKey('Credential'))
                {
                    $invokeWmiMethodParameters.Credential = $Credential
                }

                ## Query the Component Based Servicing Reg Key
                $invokeWmiMethodParameters.ArgumentList = @($hklm, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\')
                $registryComponentBasedServicing = (Invoke-WmiMethod @invokeWmiMethodParameters).sNames -contains 'RebootPending'

                ## Query WUAU from the registry
                $invokeWmiMethodParameters.ArgumentList = @($hklm, 'SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\')
                $registryWindowsUpdateAutoUpdate = (Invoke-WmiMethod @invokeWmiMethodParameters).sNames -contains 'RebootRequired'

                ## Query JoinDomain key from the registry - These keys are present if pending a reboot from a domain join operation
                $invokeWmiMethodParameters.ArgumentList = @($hklm, 'SYSTEM\CurrentControlSet\Services\Netlogon')
                $registryNetlogon = (Invoke-WmiMethod @invokeWmiMethodParameters).sNames
                $pendingDomainJoin = ($registryNetlogon -contains 'JoinDomain') -or ($registryNetlogon -contains 'AvoidSpnSet')

                ## Query ComputerName and ActiveComputerName from the registry and setting the MethodName to GetMultiStringValue
                $invokeWmiMethodParameters.Name = 'GetMultiStringValue'
                $invokeWmiMethodParameters.ArgumentList = @($hklm, 'SYSTEM\CurrentControlSet\Control\ComputerName\ActiveComputerName\', 'ComputerName')
                $registryActiveComputerName = Invoke-WmiMethod @invokeWmiMethodParameters

                $invokeWmiMethodParameters.ArgumentList = @($hklm, 'SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName\', 'ComputerName')
                $registryComputerName = Invoke-WmiMethod @invokeWmiMethodParameters

                $pendingComputerRename = $registryActiveComputerName -ne $registryComputerName -or $pendingDomainJoin

                ## Query PendingFileRenameOperations from the registry
                if (-not $PSBoundParameters.ContainsKey('SkipPendingFileRenameOperationsCheck'))
                {
                    $invokeWmiMethodParameters.ArgumentList = @($hklm, 'SYSTEM\CurrentControlSet\Control\Session Manager\', 'PendingFileRenameOperations')
                    $registryPendingFileRenameOperations = (Invoke-WmiMethod @invokeWmiMethodParameters).sValue
                    $registryPendingFileRenameOperationsBool = [bool]$registryPendingFileRenameOperations
                }

                ## Query ClientSDK for pending reboot status, unless SkipConfigurationManagerClientCheck is present
                if (-not $PSBoundParameters.ContainsKey('SkipConfigurationManagerClientCheck'))
                {
                    $invokeWmiMethodParameters.NameSpace = 'ROOT\ccm\ClientSDK'
                    $invokeWmiMethodParameters.Class = 'CCM_ClientUtilities'
                    $invokeWmiMethodParameters.Name = 'DetermineifRebootPending'
                    $invokeWmiMethodParameters.Remove('ArgumentList')

                    try
                    {
                        $sccmClientSDK = Invoke-WmiMethod @invokeWmiMethodParameters
                        $systemCenterConfigManager = $sccmClientSDK.ReturnValue -eq 0 -and ($sccmClientSDK.IsHardRebootPending -or $sccmClientSDK.RebootPending)
                    }
                    catch
                    {
                        $systemCenterConfigManager = $null
                        Write-Verbose -Message ($script:localizedData.invokeWmiClientSDKError -f $computer)
                    }
                }

                $isRebootPending = $registryComponentBasedServicing -or `
                    $pendingComputerRename -or `
                    $pendingDomainJoin -or `
                    $registryPendingFileRenameOperationsBool -or `
                    $systemCenterConfigManager -or `
                    $registryWindowsUpdateAutoUpdate

                if ($PSBoundParameters.ContainsKey('Detailed'))
                {
                    $set += [PSCustomObject]@{
                        ComputerName                     = $computer
                        ComponentBasedServicing          = $registryComponentBasedServicing
                        PendingComputerRenameDomainJoin  = $pendingComputerRename
                        PendingFileRenameOperations      = $registryPendingFileRenameOperationsBool
                        PendingFileRenameOperationsValue = $registryPendingFileRenameOperations
                        SystemCenterConfigManager        = $systemCenterConfigManager
                        WindowsUpdateAutoUpdate          = $registryWindowsUpdateAutoUpdate
                        IsRebootPending                  = $isRebootPending
                    }
                }
                else
                {
                    if($Minimal.ToBool()) {
                        $set += [PSCustomObject]@{
                            ComputerName    = $computer
                            IsRebootPending = $isRebootPending
                        }
                    } else {
                        $set += $isRebootPending
                    }
                }
            }

            catch
            {
                Write-Verbose "$Computer`: $_"
            }
        }

        # Set is addition from MH in order to streamline using Test-WindowsPendingReboot
        # to become more useful to check for the reboot and then invoke the reboot
        # for automation purposes
        if($set.Length -eq 1) {
            return $set[0]
        }

        $set;
    }
}

function ConvertTo-Hashtable() {
    [CmdletBinding()]
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Object] $InputObject 
    )

    PROCESS
    {
        if ($null -eq $InputObject) { return $null }

        if ($InputObject -is [System.Collections.IEnumerable] -and $InputObject -isnot [string])
        {
            $collection = @(
                foreach ($object in $InputObject) { 
                    ConvertTo-Hashtable $object 
                }
            )

            Write-Output -NoEnumerate $collection
        }
        elseif ($InputObject -is [psobject])
        {
            $hash = @{}

            foreach ($property in $InputObject.PSObject.Properties)
            {
                $hash[$property.Name] = ConvertTo-Hashtable $property.Value
            }

            return $hash
        }
        else
        {
            return $InputObject
        }
    }
}

function Save-InstallCheckpoint() {
    Param(
        [String] $Name,

        [String] $Value 
    )

    if($checkPoint.ContainsKey($Name)) {
        if($Value -eq $checkPoint[$Name]) {
            return $false;
        }

        $checkPoint[$Name] = $value;
        $json = $checkPoint | ConvertTo-Json -Depth 10;
        [IO.File]::WriteAllLines("$gzConfigDir/$setupName.setup.checkpoint.json", $json);
        return $true
    }

    $checkPoint.Add($Name, $Value)
    [IO.File]::WriteAllLines("$gzConfigDir/$setupName.setup.checkpoint.json", $json);
    return $True;
}
function Test-InstallCheckpoint() {
    Param(
        [Parameter(Position = 0)]
        [String] $Name, 

        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [String] $Value,

        [Parameter(ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [scriptblock] $Block 
    )

    if($checkPoint.ContainsKey($Name)) {
        if($Value -eq $checkPoint[$Name]) {
            return $true;
        }
    }

    if($Block) {
        $Block.Invoke()
        [void](Save-InstallCheckpoint -Name $Name -Value $Value)
        return; 
    }

    return $false;
}

function Out-SetupJson() {
    Param(
        [String] $Json
    )

    [IO.File]::WriteAllText("$gzConfigDir\$setupName.setup.json", $Json);
}

function Invoke-ConfigExpression() {
    Param(
        [Parameter(Position = 0)]
        [String] $Expression
    )

    if($Expression.StartsWith("https://")) {
        if($Host.Version.Major -lt 6) {
            $Expression = (Invoke-WebRequest  -Uri $uri -UseBasicParsing).Content 
        } else {
            $Expression = (Invoke-WebRequest  -Uri $uri).Content 
        }        
    }


    Invoke-Expression $Expression;
}

# only executes on first call
if($null -eq $Env:AUTORUN -or $Env:AUTORUN -ne "1") {
    if($global:AutoLoginCredentials) {
        $global:AutoLoginCredentials = Get-Credential 
    } 
    
    Invoke-WindowsUpdateScript -Uri "$($MyInvocation.ScriptName)" -Credential  $global:AutoLoginCredentials -RunOnReboot -RunName "Gainz-Setup"
    return;
}

$uri = "https://gitlab.com/nerdymishka/gainz/raw/master/scripts/gainz.setup.json"

if($Host.Version.Major -lt 6) {
    $json = (Invoke-WebRequest  -Uri $uri -UseBasicParsing).Content 
} else {
    $json = (Invoke-WebRequest  -Uri $uri).Content 
}

$json | Out-SetupJson

$config = $json | ConvertFrom-Json 

if($config.scripts -and $config.scripts.init) {
    $scripts = $config.scripts.init 
    if($scripts -is [Array]) {
        for($i = 0; $i -lt $scripts.Length; $i++) {
            $cmd = $scripts[$i];
            Test-InstallCheckpoint -Name "scripts.init[$i]" -Value $cmd -Block {
                Invoke-ConfigExpression $cmd 

                if(Test-WindowsPendingReboot) {
                    Write-Warning "Restarting computer"
                    Invoke-WindowsReboot
                    exit;
                }
            }.GetNewClosure()
        }
    }
}

if($config.powershell) {
    $sources = $config.powershell.sources 
    if($sources -is [Array]) {
        for($i = 0; $i -lt $scripts.Length; $i++) {
            $source = $sources[$i]
            if(!(Test-InstallCheckpoint -Name "powershell:sources[$i]" -Value $source)) {
                $name = $source.Name 
                $uri = $source.Uri 
                $args = @{
                    Name = $name 
                    SourceLocation = $uri 
                    PublishLocation = $source.publishUri 
                    InstallationPolicy = if($null -eq $source.policy) { "Untrusted" } else { $source.policy }
                }

                $repo = Get-PSRepository -Name $name -EA SilentlyContinue;
                if($null -ne $repo -and $repo.SourceLocation -ne $uri) {
                    Unregister-PSRepository -Name $Name 
                    Register-PSRepository @args 
                }
                if($null -eq $repo) {
                    Register-PSRepository @args 
                }

                Save-InstallCheckpoint "powershell:packages[$i]" -Value $cmd;
            }
        }
    }

    $packages = $config.powershell.packages;
    if($packages -is [Array]) {
        for($i = 0; $i -lt $scripts.Length; $i++) {
            $cmd = $packages[$i];
            if(!(Test-InstallCheckpoint -Name "powershell:packages[$i]" -Value $cmd)) {
                if($cmd -is [String]) {
                    Install-Module $cmd 
                } else {
                    $args = $cmd | ConvertTo-Hashtable
                    Install-Module @args 
                }
                Save-InstallCheckpoint "powershell:packages[$i]" -Value $cmd;
            }
        }
    }

    if(Test-WindowsPendingReboot -or $config.powershell.autoReboot) {
        Write-Warning "Restarting computer"
        Invoke-WindowsReboot
        exit;
    }
}


if($config.chocolatey)
{
    Test-InstallCheckpoint -Name "chocolatey" -Value $true  -Block {
        if($null -eq (Get-Command "Sync-Chocolatey" -EA SilentlyContinue)) {
            Install-Module Gz-ChocolateySync -Force
        }

        $ErrorActionPreference = "Stop"

        Try {
            Sync-Chocolatey -Uri "$gzConfigDir/gainz.setup.json"
        } Catch {
            Write-Error $_.Exception.ToString() 
            Write-Warning "Restarting computer"
            Invoke-WindowsReboot
            exit;
        }
    }

    if(Test-WindowsPendingReboot) {
        Write-Warning "Restarting computer"
        Invoke-WindowsReboot
        exit;
    }
}

if($config.vscode) {

    Test-InstallCheckpoint -Name "vscode:extensions" -Value $true  -Block {
        if($null -ne (Get-Command "code" -EA SilentlyContinue)) {
             
            $packages = $config.vscode.packages;
            for($i = 0; $i -lt $packages.Length; $i++) {
                code --install-extension $packages[$i]
            }
        }
    }
}


if($config.dotnetcore) {
    Test-InstallCheckpoint -Name "dotnetcore:tools" -Value $true  -Block {
        if($null -ne (Get-Command "dotnet" -EA SilentlyContinue)) {
             
            $packages = $config.dotnetcore.tools
            for($i = 0; $i -lt $packages.Length; $i++) {
                dotnet tool install $packages[$i] -g
            }
        }
    }
}

if($config.scripts -and $config.scripts.cleanup) {
    $scripts = $config.scripts.cleanup
    if($scripts -is [Array]) {
        for($i = 0; $i -lt $scripts.Length; $i++) {
            $cmd = $scripts[$i];
            Test-InstallCheckpoint -Name "scripts.cleanup[$i]" -Value $cmd -Block {
                Invoke-ConfigExpression $cmd 

                if(Test-WindowsPendingReboot) {
                    Write-Warning "Restarting computer"
                    Invoke-WindowsReboot
                    exit;
                }
            }.GetNewClosure()
        }
    }
}