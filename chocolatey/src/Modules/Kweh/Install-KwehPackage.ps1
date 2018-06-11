function Install-KwehPackage() {
    Param(
        [Parameter(Position = 0, Mandatory = $true)]
        [String] $Path
    )

    if([string]::IsNullOrWhiteSpace($Path)) {
        throw [System.ArgumentException] "Path must have a value."
    }

    if(!$Path.EndsWith(".json")) {
        throw [System.ArgumentException] "Path must be a json file."
    }

    if(! (Test-Path $Path)) {
        throw [System.IO.FileNotFoundException] "$Path"
    }

    $config = Read-KwehConfig $Path
    $context = Step-BuildContext $config
    $context | Add-Member NoteProperty "Install" $true

    if($context.OptInstall) {
        if($Env:OS -eq "Windows_NT") {
            if(!(Test-Path $Context.Opt)) {
                New-Item $Context.Opt -ItemType Directory | Write-Debug 
            }
    
            if(!(Test-Path $Context.Bin)) {
                New-Item $Context.Bin -ItemType Directory | Write-Debug
                $bin = $Context.Bin.Replace("/", "\");
                
                $isElevated = Test-KwehIsElevated 
                if($isElevated) {
                    $Path = [Environment]::GetEnvironmentVariable("Path", "Machine")
                 
                    $Path = "$Path;$bin"
                    Set-KwehEnvironmentVariable -Name "Path" -Value $Path -Scope "Machine"
                } else {
                    $Path = [Environment]::GetEnvironmentVariable("Path", "User")
                 
                    $Path = "$Path;$bin"
                    Set-KwehEnvironmentVariable -Name "Path" -Value $Path -Scope "User"
                }
            }
    
            if(!(Test-Path $Context.Var)) {
                New-Item $Context.Var -ItemType Directory | Write-Debug 
            }
    
            if(!(Test-Path $Context.Etc)) {
                New-Item $Context.Etc -ItemType Directory | Write-Debug 
            }
    
            if(!(Test-Path $Context.Log)) {
                New-Item $Context.Log -ItemType Directory | Write-Debug 
            }
    
            if(!(Test-Path $Context.Data)) {
                New-Item $Context.Data -ItemType Directory | Write-Debug 
            }

            if(!(Test-Path $Context.Tmp)) {
                New-Item $Context.Tmp -ItemType Directory | Write-Debug 
            }
        }
    }

    $context | Step-BeforeInstall $config 
    $context | Step-Install $config 
    $context | Step-AfterInstall $config 
}