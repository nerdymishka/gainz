
$tools = Get-ToolsLocation

if((Test-Path "$tools/kryptos")) {
    Remove-Item "$tools/kryptos" -Force -Recurse
}
New-Item "$tools/kryptos" -ItemType Directory;

Expand-Archive "$PSScriptRoot/kryptos.windows.zip" "$tools/kryptos" -Force


Install-BinFile `
  -Name "kryptos" `
  -Path "$tools/kryptos/Kryptos.exe" 

RefreshEnv.cmd