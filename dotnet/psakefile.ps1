
Properties {
    $MySql64Uri = "https://cdn.mysql.com//Downloads/MySQL-8.0/mysql-8.0.12-winx64.zip"
    $ToolsDir = "$PSScriptRoot/opt"
}

Task "Install:MySql" {
    if($ToolsDir) {
        if(-not (Test-Path $ToolsDir)) {
            New-Item $ToolsDir -ItemType Directory 
        }
    }

    #Invoke-WebRequest -Uri $MySql64Uri -UseBasicParsing -OutFile "$ToolsDir/mysql.zip" -Force
    #Expand-Archive "$ToolsDir\mysql.zip" "$ToolsDir\mysql-staging" -Force 
    #Move-Item "$ToolsDir/mysql-staging/mysql-8.0.12-winx64"  "$ToolsDir/mysql" -Force

    $cfg = "
[mysqld]
basedir=`"$ToolsDir/mysql/bin`"
datadir=`"$ToolsDir/mysql/data`"
    "

    $cfg | Out-File "$ToolsDir/mysql/bin/my.ini" -Encoding "UTF8" -Force
  
    $buffer = "$ToolsDir/mysql/bin/setup.txt"
    
    Start-Process "$ToolsDir/mysql/bin/mysqld.exe" -ArgumentList "--initialize", "--console" `
         -RedirectStandardError $buffer `
         -Wait -NoNewWindow  
 

    $buffer = Get-Content "$ToolsDir/mysql/bin/setup.txt" -Raw
    $output = $buffer.Split("`n")
    foreach($line in $output) {
        if($line -match "root") {
            $last = $line.LastIndexOf(":")
            $pw = $line.Substring($last + 1)
            Write-Host $pw;
        }
    }
   
}