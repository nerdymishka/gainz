# Fmg-DocFxDocs

Create DocFx compatible documentation for PowerShell modules.

This module will create markdown files that can be used with DocFx
to generate documentation for powershell modules.

By default it generates the h1 using the title: attribute in
front matter, which is yaml inserted into the very top of a
mark down file.

The yaml front matter generator can be swapped out to use `Write-CmdletName` instead of `Write-CmdletFrontMatter`.

