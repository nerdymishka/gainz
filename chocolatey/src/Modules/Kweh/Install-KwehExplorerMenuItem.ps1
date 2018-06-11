function Install-KwehExplorerMenuItem() {
    Param(
        [Parameter(Mandatory=$true, Position=0)]
        [String] $Menu,

        [Parameter(Position=1)]
        [string] $Label,


        [Parameter(Mandatory=$false, Position=2)]
        [String] $Command,


        [Parameter(Mandatory=$false, Position=3)]
        [ValidateSet('file','directory')]
        [string] $type = "file",

        [Bool] $SkipAdminCheck 
    )

    if($IsElevated) {
        $isAdmin = $true 
    } else {
        $isAdmin = Test-IsElevated 
    }
 
    if(!$isAdmin) {
		Write-Debug "Install-KwehExplorerMenuItem called without admin rights"
		$modules = "$PSScriptPath/Kweh.psm1"

        # This should only be available running inside the chocolately process
        if($env:ChocolateyPackageName) {
            $modules = "$Env:ChocolateyInstall/helpers/chocolateyInstaller.psm1"
		}
	
        return Invoke-ElevatedPowershell -Command "Install-KwehExplorerMenuItem"`
		  -Arguments "-Menu `"$Menu`" -Label `"$Label`" -Command `"$Command`" -Type `"$file`"" `
		  -Module $modules
    }

    try {
        $key = if($type -eq "file") { "*" } else { "directory" }
      
        if(-not (Test-Path "HKCR")) {
            Install-RegistryHkcrDrive 
        }
      
        if(-not (Test-Path "HKCR:\$key\shell\$Menu")) {
            New-Item "HKCR:\$key\shell\$Menu " | Write-Debug 
        }

        Set-ItemProperty "HKCR:\$key\shell\$Menu" -Name '(Default)'  -Value "$Label"

        if(-not (Test-Path "HKCR:\$key\shell\$Menu\command")) { 
          	New-Item -Path "HKCR:\$key\shell\$menu\command" | Write-Debug 
        }
      
        Set-ItemProperty "HKCR:\$key\shell\$Menu\Command" -Name '(Default)' -Value "$command \`"%1\`"';"
    
        return 0;
    } catch {
        $errorMessage = "'$menu' explorer menu item was not created - $($_.Exception.Message)"
        Write-Warning $errorMessage
    }
}