$tools = Get-ToolsLocation

Uninstall-BinFile -Name "kryptos"

Remove-Item "$tools/kryptos" -Force -Recurse

