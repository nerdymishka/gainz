$gitlab = "https://gitlab.com/nerdymishka/gainz.git"
$github = "https://github.com/nerdymishka/gainz.git"
$vsts = "https://nerdymishka@dev.azure.com/nerdymishka/gainz/_git/gainz"
$wiki = "https://gitlab.com/nerdymishka/gainz.wiki.git"

if($null -eq (Get-Command git -EA SilentlyContinue))
{
    if([Environent]::OsVersion.Platform -eq "Win32NT")
    {
        if($null -eq (Get-Command choco -EA SilentlyContinue))
        {
            Write-Warning "git is not installed. chocolately was not found."
            return
        }

        choco install git -y
    } 
    if([Environment]::OSVersion.Platform -eq [System.PlatformID]::Unix)
    {
        $pm = $false;
        if($null -ne (Get-Command apt -EA SilentlyContinue)) {
            apt install git -y
            $pm = $true;
        }

        if($null -ne (Get-Command yum -EA SilentlyContinue)) {
            yum install git -y
            $pm = $true;
        }

        if(!$pm) {
            Write-Warning "git is not installed. yum/apt were not found."
            return;
        }
    }
    if([Environment]::OSVersion.Platform -eq [System.PlatformID]::MacOSX)
    {
        if($null -eq (Get-Command brew -EA SilentlyContinue)) {
            Write-Warning "git is not installed. brew was not found."
            return;
        }
        brew install git -y 
    }
}

if((Test-Path "$HOME/Projects")) {
    Set-Location "$Home/Projects"
}

# since pullrequests & issues will most likely come 
# from github we're setting github as origin =(

if(!(Test-Path "$Home/Projects/gainz"))
{
    git clone $gitlab gainz 
    Set-Location gainz 
    git remote add github $github
    git remote add vsts $vsts 
}


git remote set-url origin --push --add $github
git remote set-url origin --push --add $vsts 

# wiki folder is ignored by parent
git clone $wiki wiki  

if($null -ne (Get-Command dotnet.exe -EA SilentlyContinue)) {
    dotnet.exe restore ./dotnet/Gainz.sln 
} else {
    Write-Warning "install the dotnet core sdk"
}