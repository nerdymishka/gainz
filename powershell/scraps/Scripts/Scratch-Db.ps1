$MySql64Uri = "https://cdn.mysql.com//Downloads/MySQL-8.0/mysql-8.0.12-winx64.zip"
$Postgress64Uri = "https://get.enterprisedb.com/postgresql/postgresql-10.5-1-windows-x64-binaries.zip?ls=Crossover&type=Crossover"
$ToolsDir = "$PSScriptRoot/opt"

Task "Install:Postgres" {
    switch ($Env:OS) {
        "Windows_NT" {
            if($ToolsDir) {
                if(-not (Test-Path $ToolsDir)) {
                    New-Item "$ToolsDir/pgsql" -ItemType Directory -Force 
                }
            }
        
            Invoke-WebRequest -Uri $Postgress64Uri -UseBasicParsing -OutFile "$ToolsDir/postgres.zip" 
            Expand-Archive "$ToolsDir\postgres.zip" "$ToolsDir\postgres-staging" -Force 
            Move-Item "$ToolsDir/postgres-staging/pgsql"  "$ToolsDir/pgsql" -Force
        }
    }
}

Task "SetVars:Postgres" {
    if(-not ($Env:Path -match "opt\\pgsql")) {
        $Env:Path += ";$ToolsDir\pgsql";
    }

    $Env:PGDATA = "$ToolsDir\pgsql\data";
    $Env:PGDATABASE = "postgres";
    $Env:PGUSER = "postgres"
    $Env:PGPORT = 5439
    $Env:PGLOCALEDIR = "$ToolsDr\pgsql\share\locale";
}

Task "Start:Postgres" -depends "SetVars:Postgres" {
   $pgProcess = Start-Process "$ToolsDir\pgsql\bin\pg_ctl.exe" `
        -ArgumentList "-D", "$ToolsDir\pgsql\data", "-l", "logfile", "start" `
        -NoNewWindow `
        -PassThru

    Set-Variable "PgServer" -Value $pgProcess -Scope Global 
}


Task "Stop:Postgres" {
    $pgProcess = Start-Process "$ToolsDir\pgsql\bin\pg_ctl.exe" `
    -ArgumentList "-D", "$ToolsDir\pgsql\data", "-l", "logfile", "stop" `
    -NoNewWindow `
    -PassThru    
}

Task "Setup:Postgres" -depends  "SetVars:Postgres" {
    & "$ToolsDir\pgsql\bin\initdb" -U "postgres" -A trust 
}

Task "Install:MySql" {
    if($ToolsDir) {
        if(-not (Test-Path $ToolsDir)) {
            New-Item $ToolsDir -ItemType Directory 
        }
    }

    Invoke-WebRequest -Uri $MySql64Uri -UseBasicParsing -OutFile "$ToolsDir/mysql.zip" -Force
    Expand-Archive "$ToolsDir\mysql.zip" "$ToolsDir\mysql-staging" -Force 
    Move-Item "$ToolsDir/mysql-staging/mysql-8.0.12-winx64"  "$ToolsDir/mysql" -Force

 
<#
    $buffer = Get-Content "$ToolsDir/mysql/bin/setup.txt" -Raw
    $output = $buffer.Split("`n")
    foreach($line in $output) {
        if($line -match "root") {
            $last = $line.LastIndexOf(":")
            $pw = $line.Substring($last + 1)
            Write-Host $pw;
        }
    }
#> 
}

Task "Setup:MySql" {
    
    $cfg = "
[mysqld]
basedir=`"$ToolsDir/mysql/bin`"
datadir=`"$ToolsDir/mysql/data`"
    "

    $cfg | Out-File "$ToolsDir/mysql/bin/my.ini" -Encoding "UTF8" -Force
  
    $buffer = "$ToolsDir/mysql/bin/setup.txt"
    
    Start-Process "$ToolsDir/mysql/bin/mysqld.exe" -ArgumentList "--initialize-insecure", "--console" `
         -RedirectStandardError $buffer `
         -Wait -NoNewWindow  
}

Task "Start:MySql" {

    Start-Process "$ToolsDir/mysql/bin/mysqld.exe" `
        -Wait -NoNewWindow  
}

Task "Stop:MySql" {
    Start-Process "$ToolsDir/mysql/bin/mysqladmin.exe" `
        -ArgumentList "-u", "root", "shutdown" `
        -Wait -NoNewWindow  
}