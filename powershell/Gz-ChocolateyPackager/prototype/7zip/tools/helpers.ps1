
function Test-ShouldInstall64() {
    Param(
        [PsCustomObject] $Installer
    )

    return  ($null -eq $installer.x32) -or ([IntPtr]::Size -eq 8 -and $env:chocolateyForceX86 -ne $true);
}

function Get-ChocolateyPipelineState() {
    Param(
        [PsCustomObject] $ConfigFile,

        [String] $Action,

        [hashtable] $PackageParameters,

        [String] $ToolsDir
    )


    if([string]::IsNullOrWhiteSpace($ToolsDir)) {
        $ToolsDir = $env:ChocolateyPackageFolder
        if(!$ToolsDir) {  throw "ToolsDir must be specified" }

        $ToolsDir = $ToolsDir.Replace("\", "/").Trim("/")
        $ToolsDir += "/tools";
    }

    if($null -eq $PackageParameters) {
        if($null -ne (Get-Command Get-PackageParameters -EA SilentlyContinue)) {
            $PackageParameters = Get-PackageParameters
        } else {
            $PackageParameters = @{}
        }
    }

    if([string]::IsNullOrWhiteSpace($ConfigFile)) {
        $ConfigFile = Get-Item "$ToolsDir/*.nuspec.json" -EA SilentlyContinue
        if(!$ConfigFile) {
            throw "ConfigFile must be specificed or a file in the tools directory that ends with .nuspec.json must exist"
        }

        $ConfigFile = $ConfigFile.FullName.Replace("\", "/")
    }

    $wd = Split-Path $ConfigFile
    $pipelineStateFile = "$wd/state.json";
    $pipeline = $null;
    if(Test-Path $pipelineStateFile) {
        $pipeline = Get-Context $pipelineStateFile -Raw | ConvertFrom-Json
        return $pipeline; 
    }

    $config = Get-Content $ConfigFile -Raw | ConvertFrom-Json

    if($config.metadata) { $metadata = $config.metadata }
    $tmpDir = $Env:Temp.Replace("\", "/").Trim("/")
    if($metadata.id) {
        $tmpDir += "/$($metadata.id)"
    }
    if($metadata.version) {
        $tmpDir += "/$($metadata.version)"
    }

    $pipeline = [PsCustomObject]@{
        packageName = $metadata.id 
        version = $metadata.version
        status = "new"
        packageType = $null
        lastAction = $action 
        state = [PsCustomObject] {
            installed = $false,
            exception = $null 
            targetDir = $null 
            packageParameters = $PackageParameters
            steps = @(
                "Get-ChocolateyPipelineState"
            )
            action = $action
            toolsDir = $wd
            is64bit = $null
            isLocalFile = $false 
            tmpDir = $tmpDir 
            installCmd = $null
            uninstallCmd = $null
            splat = @{}
            silentArgs = $null;
        }
        config = [PsCustomObject] {
            path = $ConfigFile 
            data = $config 
        }
        installer = $null 
        cleanup = $null 
        stateUri = $pipelineStateFile
    }

    $pipeline | ConvertFrom-Json -Depth 10 | Out-File $pipelineStateFile -Encoding "UTF8" -Force 
    return $pipeline;
}

function Step-ExpandPipelineVariable() {
    Param(
        [Parameter(ValueFromPipeline = $true)]
        [PsCustomObject] $Pipeline
    )

    $state = $Pipeline.state;
    $state.steps += "[Call]: Step-ExpandPipelineVariables"
    $config = $Pipeline.config.data;
    $gz = $config.gzInstructions;
    $pp = $Pipeline.state.packageParameters
    $uninstall = $state.action -eq "uninstall" -or $state.action -eq "rm";

    try {



        $validPackageTypes = @("installer", "archive", "custom")

        $installer = $null;
        $cleanup = $null;

        $packageType = "installer";
        if($uninstalling) { 
            $packageType = $state.packageType 
        } else {
            if($pp)
            {
                $name = $packageTypeParameter = $pp.Keys -match "packageType";
                if($name) {
                    $packageType = $pp[$name];
                }
            }

            if(! ($validTypes -match $packageType)) {
                throw new "Invalid package type: $packageType"
            }
        }


        $installer = $gz[$packageType];
        if(!$installer) {
           
            foreach($type in $validPackageTypes) {
                $installer = $update[$type];
                if($installer) {
                    Write-Debug "$packageType does not exist, falling back to $type"
                    $packageType = $type;
                    
                }
            } 
        }

        $state.packageType = $packageType;
        $pipeline.installer = $installer;
        $cleanup = $pipeline.cleanup = $installer.cleanup;
        $preScripts = $null;
        $postScripts = $null;
        $actionScripts = $null;

        $targetDir = $state.targetDir;
        if(!$uninstalling) {
            $name = $pp.Keys -match "targetDir"
            if($name) {
                $targetDir = $pp["targetDir"];
                $state.targetDir = $targetDir;
            }
        }

        $pipeline.state.silentArgs = $installer.silentArgs;

        if($packageType -eq "archive") {
            if(!$targetDir) {
                $toolsLocation = $Env:ChocolateyToolsLocation
                if(!$toolsLocation) {
                    Write-Debug "chocolateyToolsLocation is empty, falling back to toolsDir"
                    $toolsLocation = $state.toolsDir;
                }

                $toolsLocation = $toolsLocation.Replace("\", "/");
                $targetDir = "$toolsLocation/$($pipeline.packageName)";
                $state.targetDir = $targetDir;
            }
        }

        $pipeline.state = $state;
        $pipeline.installer = $installer;
        $pipeline.cleanup = $cleanup;

        Invoke-ResolvePackageParameters -Pipeline $Pipeline
        
        $state = $pipeline.state;

        $splat = @{
            packageName = $pipeline.packageName 
        }

       
        # if the installer packages are embedded
        if($gz.embed) {
            if(Test-ShouldInstall64 -Installer $installer) {
                $splat.file = $installer.x64.file 
                $result.hash = $installer.x64.hash 
                $result.algo = $installer.x64.algo 
            } else {
                $splat.file = $installer.x32.file
                $result.hash = $installer.x32.hash 
                $result.algo = $installer.x32.algo 
            }

            if($packageType -eq "archive") {
                $state.installCmd = "Get-ChocolateyUnzip"
                $state.uninsallCmd = "Uninstall-ChocolateyZipPackage"
                $splat.Destination = $targetDir;
                
            } elseif($packageType -eq "installer") {
                $state.installCmd = "Install-ChocolateyInstallPackage";
                $state.uninsallCmd = "Uninstall-ChocolateyPackage";               
                $splat.validExitCodes = $installer.exitCodes 
                $splat.silentArgs = $state.silentArgs
                $splat.fileType = $installer.type;
                if($installer.grep) {
                    $splat.softwareName = $installer.grep
                }
            } else {
                $state.installCmd = $installer.scripts.install;
                $state.uninsallCmd = $cleanup.scripts.uninstall;  
            }
        } else {
            $splat.url = $installer.x32.uri 
            $splat.url64 = $installer.x64.uri 
            $splat.checksum = $installer.x32.hash 
            $splat.checksum64 = $installer.x64.hash 
            $splat.checksumType = $installer.x32.algo
            $splat.checksumType64 = $installer.x64.algo 

            if($update.cdn -and $update.cdn.tokenParameter) {
                $name = $update.cdn.tokenParemeter;
                $value = $Env:$name 
                if(!$value) {
                    throw "missing token $value"
                }
                
                if($splat.url)
                {
                    if(!$splat.url.Contains("?"))
                    {
                        $splat.url += "?"
                    } else {
                        $splat.url += "&"
                    }

                    $splat.url += "$name=$value"
                }

                if($splat.url64)
                {
                    if(!$splat.url64.Contains("?"))
                    {
                        $splat.url64 += "?"
                    } else {
                        $splat.url64 += "&";
                    }

                    $splat.url64 += "$name=$value"
                }
            }

            if($packageType -eq "archive") {
                $state.installCmd = "Install-ChocolateyZipPackage"
                $state.UninstallCmd = "Uninstall-ChocolateZipPackage"
                $extractDir = $installer.extractSubDirectory
                if($extract) {
                    $splat.SpecificFolder = $extractDir
                }          
            } elseif($packageType -eq "installer") {
                $state.installCmd = "Install-ChocolateyPackage"
                $state.uninstallCmd = "Uninstall-ChocolateyPackage"
                $splat.validExitCodes = $installer.exitCodes 
                # silentArgs may change due to parameters
                $splat.silentArgs = $state.silentArgs
                $splat.fileType = $installer.type;
                if($installer.grep) {
                    $splat.softwareName = $installer.grep
                }
                
            } else {
                $state.installCmd = $installer.scripts.install;
                $state.uninstallCmd = $installer.scripts.uninstallCmd;
            }
        }
        $pipeline.state = $state;
        return $pipeline;
    } catch {
        $pipeline.state = $state;
        if($_.Exception)
        {
            Write-LastException $Config $Exception
            throw $_.Exception 
        }       
    } finally {
        $pipeline | ConvertFrom-Json -Depth 10 | Out-File $pipelineStateFile -Encoding "UTF8" -Force 
    }
}

function Step-ExecuteScripts() {
    Param(
        [Parameter(ValueFromPipeline)]
        [PsCustomObject] $Pipeline,

        [Parameter(Position = 0)]
        [String[]] $Scripts,

        [String] $Step
    )

    if(!$scripts -or $scripts.Length -eg 0) {
        return;
    }

    $state = $Pipeline.state;
    $state.steps += "[call] Step-ExecutePipeline ($Step)"
    $model = $Pipeline | Get-TemplateModel
    Set-Variable -Name "Pipeline" -Value $Pipeline -Scope Global
    foreach($script in $scripts) {
        $file  = Resolve-TemplatePath -Path $script -Mode $Model
        if(!(Test-Path $file)) {
            Write-Debug "Could not locate $file"
            continue;
        }

        . "$script"
    }

    $gPipeline = Get-Variable -Name "Pipeline" -Scope Global 

    $pipeline = $gPipeline.Value;

    if($gPipeline) {
        $gPipeline | Remove-Variable;
    }

    return $pipeline;
}

function Step-ExecutePipeline() {
    Param(
        [Parameter(ValueFromPipeline= $true)]
        [PsCustomObject] $Pipeline,
    )

    $state = $Pipeline.state;
    $state.steps += "[Call]: Step-ExecutePipeline"
    $config = $Pipeline.config.data;
    $gz = $config.gzInstructions;
    $uninstall = $state.action -eq "uninstall" -or $state.action -eq "rm";
    $installer = $Pipeline.installer 
    $cleanup = $Pipeline.cleaup;
    $beforeScripts = $null;
    $afterScripts = $null;
    $actionScripts = $null;
    $scripts = if($uninstall) {$cleanup.scripts} else { $installer.scripts }

    if($scripts)
    {
        $before = $null;
        if($scripts.pre) {
            $before = $scripts.pre 
        } elseif($scripts.before) {
            $before = $scripts.before 
        }

        if($scripts.post) {
            $after = $scripts.after;
        } elseif($scripts.after) {
            $after = $scripts.after;
        }

        if($uninstall) {
            $actionScripts = $scripts.uninstall; 
        } else {
            $actionScripts = $scripts.install;
        }
    } 

    if(!$uninstall)
    {
        if($beforeScripts) {
            $Pipeline = $Pipeline | Step-ExecuteScripts $beforeScripts -Step "Before Install"
        }

        switch($state.packageType)
        {
            "custom" {
                if($actionScripts) {
                    $Pipeline = $Pipeline | Step-ExecuteScripts $actionScripts -Step "Custom Install"
                } 
            }
            default {
                $state = $pipeline.state;
                $install = $state.installCmd 
                $state.steps += "[call] $install"
                $splat = $state.splat
                & $install @splat 

                if(!$state.targetDir -and $null -ne (Get-Command Get-AppInstallLocation -EA SilentlyContinue)) {
                    $state.targetDir = Get-AppInstallLocation $installer.grep
                }

                $pipeline.state = $state;
            }
        }

        if($afterScripts) { 
            $Pipeline = $Pipeline | Step-ExecuteScripts $afterScripts -Step "After Install"
        }


        $Pipeline | Step-UpgradeFileOperations 
    } else {
        $Pipeline | Step-UninstallFileOperations 

        if($beforeScripts) {
            $Pipeline = $Pipeline | Step-ExecuteScripts $beforeScripts -Step "Before Uninstall"
        }

        switch($state.packageType)
        {
            "custom" {
                if($actionScripts) {
                    $Pipeline = $Pipeline | Step-ExecuteScripts $actionScripts -Step "Uninstall"
                } 
            }
            default {
                $state = $pipeline.state;
                $install = $state.installCmd 
                $state.steps += "[call] $install"
                $splat = $state.splat
                & $install @splat 

                if(!$state.targetDir -and $null -ne (Get-Command Get-AppInstallLocation -EA SilentlyContinue)) {
                    $state.targetDir = Get-AppInstallLocation $installer.grep
                }

                $pipeline.state = $state;
            }
        }

        if($afterScripts) { 
            $Pipeline = $Pipeline | Step-ExecuteScripts $afterScripts -Step "After Uninstall"
        }
    }
}

function Write-ChocolateyPipelineException() {
    Param(
        [Parameter(ValueFromPipeline = $true)]
        [PsCustomObject] $Pipeline,

        [Parameter(Position=0)]
        [Exception] $Exception 
    )

    $pipeline.state.exception = [PsCustomObject] @{
        message = $Exception.Message 
        stack = $Exception.StackTrace
        type = $Exception.GetType().FullName  
    }

    $outFile = $pipeline.stateUri;

    $config | ConvertFrom-Json -Depth 10 | Out-File "$outFile" -Encoding "UTF8"
}

function Invoke-GzChocolateyPackage() {
    Param(
        [Parameter(Position = 0)]
        [string] $Action = "Install",
        
        [Parameter(Position = 1)]
        [string] $ConfigFile,

        [hashtable] $PackageParameters,
        
        [string] $ToolsDir,
    )

    [PsCustomObject] $pipeline = $null;
    try {
        $pipeline = Get-ChocolateyPipelineState -ConfigFile $ConfigFile `
            -Action $Action `
            -PackageParameters $PackageParameters `
            -ToolsDir = ToolsDir 
        $pipeline = $pipeline | Step-ExpandPipelineVariable 
        $pipeline | Step-ExecutePipeline
    } catch {
        if($pipeline -and $_.Exception) {
            $pipeline | Write-ChocolateyPipelineException
            throw $_.Exception 
        }
    } finally {
        if($pipeline) {
            $pipeline | ConvertTo-Json -Depth 10 | Out-File "$($pipeline.stateUri)" -Encoding "UTF8" -Force 
        }
    }
}



function Resolve-DefaultPackageParameters() {
    Param(
        [Parameter(ValueFromPipeline = $true)]
        [PsCustomObject] $Pipeline
    )

    $state = $Pipeline.state;
    $install = $state.action -eq "install";
    $silentArgs = "";
    $parameters = $null;
    
    if($install) { 
        $silentArgs = $pipeline.installer.silentArgs
        $parameters = $pipeline.installer.parameters 
    } else {
        $silentArgs = $pipeline.cleanup.silentArgs
        $parameters = $pipeline.cleanup.parameters;
        if(!$silentArgs) {
            $silentArgs = $pipeline.installer.silentArgs
        }
        if(!$parameters) {
            $parameters = $pipeline.installer.parameters;
        }
    }

    if(!$parameters) {
        return;
    }

    $decryptionKey = Get-Variable -Name "GZ_CHOCOLATEY_DECRYPT_KEY" -Scope Global
    if(!$decryptionKey) {
        $decryptionKey = $Env:GZ_CHOCOLATEY_DECRYPT_KEY
    } 
    if($decryptionKey -is [string]) {
        $decryptionKey = [System.Text.Encoding]::UTF8.GetBytes($decryptionKey)
    }
    $canDecrypt = $decryptionKey -ne $null;
    foreach($key in $PackageParameters) {
        if(!$parameters[$key]) {
            continue;
        }

        $set = $parameters[$key];
        if($set.type -eq "flag") {
            $silentArgs += " " + $set.format;
            continue;
        }
        $value = $pp[$key];
        if($set.type -eq "encrypted") {
            if(!$canDecrypt) {
                Write-Warning "Can't decrypt $key"
                continue;
            }

            $value = Unprotect-String $value $decryptionKey
        }
        $silentArgs += " " + ($set.format -f $value);
    }

    $s = $installer.silentArgs;
    $args = ($s + " " + $silentArgs).Trim();
    $state.silentArgs = $args;
    $pipeline.state = $state;
}


function Invoke-ResolvePackageParameters() {
    Param(
        [Parameter(ValueFromPipeline = $true)]
        [PsCustomObject] $Pipeline
    )

    if($null -ne (Get-Command Resolve-PackageParameters -EA SilentlyContinue)) {
        Resolve-PackageParameters -Pipeline $Pipeline
    } else {
        Resolve-DefaultPackageParameters -Pipeline $Pipeline
    }
}

function Resolve-DefaultPackageParameters() {
    Param(
        [Parameter(ValueFromPipeline = $true)]
        [PsCustomObject] $Pipeline
    )

    $installer = $Pipeline.installer;
    $pp = $Pipeline.state.packageParameters
    if(!$pp) {
        return;
    }

    $parameters = $Installer.parameters;
    $silentArgs = "";
    foreach($key in $pp.Keys) {
        if(!$parameters[$key]) {
            continue;
        }

        $set = $parameters[$key];
        if($set.type -eq "flag") {
            $silentArgs += " " + $set.format;
            continue;
        }
        $silentArgs += " " + ($set.format -f $pp[$key]);
    }

    $s = $installer.silentArgs;
    $args = ($s + " " + $silentArgs).Trim();
    $pipeline.state.silentArgs = $args;
}


function Resolve-Template() {
    Param(
        [Parameter(Position = 0)]
        [String] $Template,

        [Parameter(Position = 1, ValueFromPipeline = $True)]
        [PsCustomObject] $Model 
    )

    $eval = {  
        param($m) 
        
        $var = $m.Value.TrimStart('{').TrimEnd('}').Trim()
        if($var -match "env:") {
            $parts = $var.Split(':')
            $name = $parts[1];
            return (Get-Item "Env:/$Name").Value 
        }
        if($var.Contains(".")) {
            $parts = $var.Split(".")
            $m = $Model;
            for($i = 0; $i -lt $parts.Length; $i++) {
                $next = $parts[$i]
                $m = $m.$next 
                if($null -eq $m) {
                    return $null;
                }
            }
            return $m;
        }
        $m = $Model.$var 
        return $m 
    }

    $pattern = [Regex]"{{\s*[\w\.:]+\s*}}"
   
    $result = $pattern.Replace($Template, $eval)
 
    return $result
}


function Step-UpgradeFileOperations() {
    Param(
        [Parameter(ValueFromPipeline = $true)]
        [PsCustomObject] $Pipeline
    )


    $installer = $Pipeline.installer;
    $state = $Pipeline.state;
    $state.steps += "[call] Step-UpgradeFileOperations"

    if(!$installer -or !$installer.fs) {
        return;
    }

    $model = $Pipeline | Get-TemplateModel

    if($installer.fs.rm)
    {
        foreach($file in $installer.fs.rm) {
            $pattern = Resolve-TemplatePath -Path $file -Model $model 
            if(Test-Path $pattern) {
                Remove-Item $pattern -Force -Recurse | Write-Debug
            }
        }
    }   

    if($installer.fs.cp) {
        foreach($set in $installer.fs.cp) {
            $src = Resolve-TemplatePath -Path $set.src -Model $model 
            $dest = Resolve-TemplatePath -Path $set.dest -Model $model 

            if(Test-Path $src)
            {
                if(!Test-Path $dest) {
                    New-Item $dest -ItemType Directory | Write-Debug 
                }

                if($set.recurse) {
                    Copy-Item $src $dest -Recurse -Force | Write-Debug 
                } else {
                    Copy-Item $src $dest -Force | Write-Debug 
                }
            }            
        }
    }

    

    if($installer.fs.mv)
    {
         foreach($set in $installer.fs.mv) {
            $src = Resolve-TemplatePath -Path $set.src -Model $model 
            $dest = Resolve-TemplatePath -Path $set.dest -Model $model 

            if(Test-Path $src)
            {
                if(!Test-Path $dest) {
                    New-Item $dest -ItemType Directory | Write-Debug 
                }

                if($set.recurse) {
                    Move-Item $src $dest -Recurse -Force | Write-Debug 
                } else {
                    Move-Item $src $dest -Force | Write-Debug 
                }
            }            
        }
    }
    

    if($install.fs.ignore)
    {
        foreach($file in $installer.fs.ignore) {
            $pattern = Resolve-TemplatePath -Path $file -Model $model 
            if(Test-Path $pattern) {
                $ignore = "$pattern.ignore"
                New-Item $ignore -Value "" | Write-Debug
            }
        }
    }

    if($installer.fs.binFiles)
    {
        foreach($set in $installer.fs.binFiles) {
            $name = $set.Name
            $src = $set.Src 
            $src = Resolve-TemplatePath -Path $src -Model $model 
            if(Test-Path $pattern) {
                Install-BinFile $name $src | Write-Debug
            }
        }
    }

   
}

function Get-TemplateModel() {
    Param( 
        [Parameter(ValueFromPipeline = $true)]
        [PsCustomObject] $Pipeline
    )
    
    $state = $Pipeline;


    return [PsCustomObject]@{
        "apps" =  $Env:ChocolateyToolsLocation
        "toolsDir" =  $state.toolsDir 
        "targetDir" = $state.targetDir
        "home" = $env:HOME.Replace("\", "/")
        "public" = $env:Public.Replace("\", "/")
        "programFiles" = $env:programFiles
        "programFiles86" = ${env:ProgramFiles(x86)}
        "programData" = $env:ALLUSERSPROFILE
        "allUsersProfile" = $env:ALLUSERSPROFILE
    }
}

function Resolve-TemplatePath() {
    Param(
        [String] $Path,

        [PsCustomObject] $Model 
    )

    if($Path.StartsWith("./") -or $Path.StartsWith("~/")) {
        return ($Model.toolsDir) + $file.Substring(1);
    }

    return Resolve-Template -Template $Path -Model $model
}


function Invoke-UninstallFileOperations() {
    Param(
        [Parameter(ValueFromPipeline = $true)]
        [PsCustomObject] $Pipeline
    )

    $model = $Pipeline | Get-TemplateModel
   
    if($installer.fs.cp)
    {
        foreach($set in $installer.fs.cp) {
            $rm = $set.rm
            $dest = Resolve-TemplatePath -Path $set.dest -Model $model 
            if($rm -and Test-Path $dest) {
                Remove-Item $dest -ItemType Directory | Write-Debug 
            }
        }
    }     


    if($installer.fs.cp)
    {
        foreach($set in $installer.fs.cp) {
            $rm = $set.rm
            $dest = Resolve-TemplatePath -Path $set.dest -Model $model 
            if($rm -and Test-Path $dest) {
                Remove-Item $dest -ItemType Directory | Write-Debug 
            }
        }
    }
    
    if($installer.fs.mv)
    {

        foreach($set in $installer.fs.mv) {
            $rm = $set.rm 
            $dest = Resolve-TemplatePath -Path $set.dest -Model $model 
            if($rm -and Test-Path $dest) {
                Remove-Item $dest -Force -Recurse | Write-Debug 
            }
        }
    }

    
    if($installer.fs.ignore)
    {
        foreach($file in $installer.fs.ignore) {
            $pattern = Resolve-TemplatePath -Path $file -Model $model 
            $ignore = "$pattern.ignore"
            if(Test-Path $ignore) {
                Remove-Item $ignore -Force | Write-Debug
            }
        }
    }

    if($installer.fs.binFiles)
    {
        foreach($set in $installer.fs.binFiles) {
            $name = $set.Name
            
            if(Test-Path $pattern) {
                Uninstall-BinFile $name | Write-Debug
            }
        }
    }
}

