

function Invoke-GzWinTelemetry() {
    Param(
        [Switch] $IncludeFiles,

        [Switch] $IncludeAdmins,

        [Switch] $IncludeLocalMembership,

        [Switch] $IncludeBitLockerStatus,

        [Switch] $IncludeChrome,

        [Switch] $IncludePowershell,

        [Switch] $IncludeCloudJoin,

        [Switch] $IncludeWin32,

        [Switch] $IncludeAppx,

        [Switch] $IncludeEnabledFeatures,

        [Switch] $IncludeServices
    )
    $elevated = Test-GzCurrentUserIsElevated
    if(!$elevated)
    {
        Write-Warning "Invoke-GzWinTelemetry requires admin rights.";
        return;
    }

    $computerName = $env:COMPUTERNAME
    if($computerName) {
        $computerName = $computerName.Trim()
    }

    $hash = $null;
    $devDetail = (Get-WMIObject -ComputerName $computerName -Credential $Credential -Namespace root/cimv2/mdm/dmmap -Class MDM_DevDetail_Ext01 -Filter "InstanceID='Ext' AND ParentID='./DevDetail'")
    if ($devDetail -and (-not $Force))
    {
        $hash = $devDetail.DeviceHardwareData
    }

    $serial = (Get-WmiObject -ComputerName $computerName -Credential $Credential -Class Win32_BIOS).SerialNumber.Trim()


    $telemetry = [PsCustomObject] @{
        hash = $hash
        serial = $serial
        computerName = $computerName 
        domain = $env:USERDOMAIN

        chrome = [PSCustomObject]@{
            extensions = $null
        }
        windows = [PSCustomObject]@{
            version = [System.Environment]::OSVersion.Version.ToString()
            win32App = $null 
            bitLocker = $null
            localUsers = $null
            localGroups = $null
            admins = $null
            appx = $null 
            enabledFeatures = $null 
            services = $null 
            volumes = $null 
            cloudJoinUsers = $null 
            scheduledTasks = $null 
        }
       
        powershell = [PSCustomObject]@{
            modules = $null
            version = $Host.Version.ToString()
        }
       
    }

    $win = $telemetry.windows 
    $posh = $telemetry.powershell;

    $win.win32App = Read-GzWin32App 
    if($win.win32App -and !($win.win32App -is [Array])) {
        $win.win32App = @($win.win32App)
    }
    
    if($IncludeAppx.ToBool()) {
        $win.appx = Read-GzWinAppxPackage 
        if($win.appx -and !($win.appx -is [Array])) {
            $win.appx = @($win.appx)
        }    
    }
    
    if($IncludeServices)
    {
        $win.services = Read-GzWinService

    }
   
    $win.volumes = Read-GzWinVolume 
    if($win.volumes -and !($win.volumes -is [Array])) {
        $win.volumes = @($win.volumes)
    }

    if($IncludeEnabledFeatures)
    {
        $win.enabledFeatures = Read-GzWinEnabledFeature 
        if($win.enabledFeatures -and !($win.enabledFeatures -is [Array])) {
            $win.enabledFeatures = @($win.enabledFeatures)
        }
    }
    

    $win.scheduledTasks = Read-GzWinScheduledTask 
    if($win.scheduledTasks -and !($win.scheduledTasks -is [Array])) {
        $win.scheduledTasks = @($win.scheduledTasks)
    }

    if($IncludePowershell.ToBool()) {
        $posh.modules = Read-GzPowershellModule 
        if($posh.modules -and !($posh.modules -is [Array])) {
            $posh.modules = @($posh.modules)
        }
    }
   

    if($IncludeAdmins.ToBool()) {
        $win.admins = Read-GzWinAdministratorMember 
        if($win.admins -and !($win.admins -is [Array])) {
            $win.admins = @($win.admins)
        }
    }

    if($IncludeLocalMembership.ToBool()) {
        $win.localGroups = Read-GzWinLocalGroup 
        
        $win.localUsers = Read-GzWinLocalUser 
    }

    if($IncludeBitLockerStatus.ToBool()) {
        $win.bitLocker = Read-GzWinBitLockerStatus 
    }

    if($IncludeChrome.ToBool()) {
        $telemetry.chrome.extensions = Read-GzWinChromeExtension -All
        if($telemetry.chrome.extensions -and !($telemetry.chrome.extensions -is [Array])) {
            $telemetry.chrome.extensions = @($telemetry.chrome.extensions)
        }
    }

    if($IncludeCloudJoin.ToBool()) {
        $win.cloudJoinUsers = Get-GzWinCloudJoinUser 
        if($win.cloudJoinUsers -and !($win.cloudJoinUsers -is [Array])) {
            $win.cloudJoinUsers = @($win.cloudJoinUsers)
        }
    }

    return $telemetry
}