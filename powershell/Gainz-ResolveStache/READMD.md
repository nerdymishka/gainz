# Gainz Resolve-Stache

A simple mustache like string template resolver. Resolve-Stache is useful for string templates
that reside in files or external sources where powershell string interpolation cannot be used.  

```powershell
$content = Resolve-Stache "Hello, {{ Person }}"  -Model [PsCustomObject]@{Person = "Parker"}
Write-Host $content;
```