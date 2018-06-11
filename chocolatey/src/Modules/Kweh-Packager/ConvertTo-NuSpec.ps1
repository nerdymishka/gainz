<#
Copyright 2016 Nerdy Mishka LLC

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
#>

function ConvertTo-NuSpec() {
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [PsObject] $InputObject,

        [string] $RelativePath,
        
        [switch] $Chocolatey 
    )

    #TODO: new nuspecs don't need a schema
    
    $config = $InputObject;
    
    $xmlns = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"
    if($Chocolatey.ToBool()) {
        $xmlns = "http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd"
    }

   
    $xmlString ="<?xml version=`"1.0`" encoding=`"utf-8`"?>
<package xmlns=`"$xmlns`">
  <metadata>
  </metadata>
</package>"
    
   
    $nuspec = [xml]$xmlString 
    $parentDirectory = $OutFile
    if([string]::IsNullOrWhitespace($parentDirectory)){
         $parentDirectory = $Path 
    }
  
    
    $ns = New-Object System.Xml.XmlNamespaceManager($nuspec.NameTable)
    $ns.AddNamespace("ns", $nuspec.DocumentElement.NamespaceURI) | Out-Null
    $metadataElement = $nuspec.SelectSingleNode("//ns:metadata", $ns)
     
    $properties = @(
        "id", 
        "version", 
        "title",
        "authors",
        "owners",
        "licenseUrl",
        "requireLicenseAcceptance",     #boolean
        "developmentDependency"         #boolean
        "summary",
        "copyright",
        "language",
        "tags",
        "serviceable",                   #boolean
        "packageTypes",
        "dependencies",
        "frameworkAssemblies",
        "references",
        "contentFiles",
        "files"
     )

     # Chocolatey First Wave https://github.com/chocolatey/choco/issues/205
     $chocolateyProperties = @(
          "projectSourceUrl", 
          "packageSourceUrl",
          "docsUrl",
          "mailingListUrl",
          "bugTrackerUrl",
          "replaces",
          "provides",
          "conflits"
     )

     if($chocolatey.ToBool()) {
         foreach($element in $chocolateyProperties) {
             $properties += $element;
         }
     }

     $arrayProperties = @("tags", "authors", "owners");
     $textProperties = @("description", "releaseNotes");
     $collectionProperties = @("dependencies", "references", "files", "packageTypes", "contentFiles", "frameworkAssemblies")
     
     # Add text properties last
     foreach($textProperty in $textProperties) {
         $properties += $textProperty;
     }

     $metadata = $config.Metadata;

     foreach($propertyName in $properties) {
         
         $configEntryValue = $metadata.$propertyName

         if($configEntryValue -eq $null) {
             Write-Debug "$propertyName has no value";
             continue;
         }

         if($textProperties.Contains($propertyName)) {
            $text = [string] $configEntryValue
          
            $textElement = $nuspec.CreateElement($propertyName, $xmlns);
            $isRelativePath = $text.StartsWith("~/") -or $text.StartsWith("./") -or $text.StartsWith(".\")
            if($isRelativePath -or $text.EndsWith(".md") -or $text.EndsWith(".txt")) {
                
                if($isRelativePath) {
                        if([string]::IsNullOrWhiteSpace($RelativePath)) {
                            $wd = $RelativePath
                        } else {
                            $wd = (Get-Location).Path
                        }
                        
                        $text = Join-Path "$wd" ($text.Substring(2));
                }
                
                $text = [System.IO.File]::ReadAllText($text);
            } 

            
            
            $textElement.AppendChild($nuspec.CreateTextNode($text)) | Out-Null;
            $metadataElement.AppendChild($textElement) | Out-Null;
            
            continue;
         }

          if($propertyName -eq "frameworkAssembly") {
             $continue;
         }
         
         if($arrayProperties.IndexOf("$propertyName") -gt -1) {
            $value = $configEntryValue
            if($configEntryValue -is [Array]) {
                $delimiter = ','
                if($propertyName -eq "tags") {
                    $delimiter = ' ';
                }
                $value = [string]::Join($delimiter, $value)
            }
            $arrayElement = $nuspec.CreateElement($propertyName, $xmlns)
            $textNode = $nuspec.CreateTextNode($value);
            $arrayElement.AppendChild($textNode) | Out-Null;
            $metadataElement.AppendChild($arrayElement) | Out-Null;
            
            continue;
         }

         if(!$collectionProperties.Contains($propertyName)) {
            $element = $nuspec.CreateElement($propertyName, $xmlns);
            $value = $configEntryValue;
            if($value -is [System.Boolean]) {
                $value = $value.ToString().ToLower();
            }

            $element.AppendChild($nuspec.CreateTextNode($value)) | Out-Null;

            $metadataElement.AppendChild($element)| Out-Null;
            continue;
         }

         if($propertyName -eq "dependencies") {
            $dependenciesElement = $nuspec.CreateElement("dependencies", $xmlns);

         
            $configEntryValue | Get-Member -Type NoteProperty | ForEach-Object {
                $name = $_.Name
                $value = $configEntryValue.$name 
                if($value.dependencies) {
                    $value | Get-Member -Type NoteProperty | ForEach-Object {
               
                        $groupElement = $nuspec.CreateElement("group", $xmlns);
                        $groupName = $name;
                        
                        if($groupName -ne "default") {
                            $groupElement.SetAttribute("targetFramework",  $groupName);
                        }
                        
                        $dependencies = $configEntryValue.$groupName;
                        if($dependencies.dependencies) {
                            $group = $dependencies;
                            $dependencies = $group.dependencies;
                        }
                        $dependencies | Get-Member -Type NoteProperty | ForEach-Object { 
                            
                            $dependencyElement = $nuspec.CreateElement("dependency", $xmlns);
                            $id = $_.Name;
                            $version = $dependencies.$id;
                            $dependencyElement.SetAttribute("id", $id);
                            $version = $dependencies.$id;
                            if(-not [string]::IsNullOrWhiteSpace($version) -and $version -ne "*"){
                                $dependencyElement.SetAttribute("version", $version);
                            }
                            
                            $groupElement.AppendChild($dependencyElement)| Out-Null;
                        }
                    }
                     
                    $dependenciesElement.AppendChild($groupElement)| Out-Null;
                } else {
                
                    $dependencyElement = $nuspec.CreateElement("dependency", $xmlns);
                    $dependencyElement.SetAttribute("id", $name);
                    if(-not [string]::IsNullOrWhitespace($value)) {
                        $dependencyElement.SetAttribute("version", $value);
                    }
                    $dependenciesElement.AppendChild($dependencyElement) | Out-Null;
                }
            }
         
            $metadataElement.AppendChild($dependenciesElement) | Out-Null;
         }

        

         if($propertyName -eq "frameworkAssemblies") {
             
             $frameworkAssembliesElement = $nuspec.CreateElement("frameworkAssemblies", $xmlns);
             $configEntryValue | Get-Member -Type NoteProperty | ForEach-Object {
                 $name = $_.Name 
                 $value = $configEntryValue.$name

                 $frameworkAssemblyElement = $nuspec.CreateElement("frameworkAssembly", $xmlns);
                 $frameworkAssemblyElement.SetAttribute("assemblyName", $name);
                 if($value) {
                     $frameworkAssemblyElement.SetAttribute("targetFramework", $value);
                 }

                 $frameworkAssembliesElement.AppendChild($frameworkAssemblyElement) | Out-Null
             }
            
            $metadataElement.AppendChild($frameworkAssembliesElement) | Out-Null
             
         }

         if($propertyName -eq "references") {
            $referencesElement = $nuspec.CreateElement("references", $xmlns);
            
            if($configEntryValue -is [Array]) {
                foreach($reference in $value.references) {
                    $referenceElement = $nuspec.CreateElement("reference", $xmlns);
                    $referenceElement.SetAttribute("file", $reference)
                    $referencesElement.AppendChild($referenceElement) | Out-Null
                }
                continue;
            }
         
            $configEntryValue | Get-Member -Type NoteProperty | ForEach-Object {
                $name = $_.Name
                $value = $configEntryValue.$name 

                if($value.references) {
                    $groupElement = $nuspec.CreateElement("group", $xmlns);
                    $groupName = $_.Name;
                    
                    if($groupName -ne "default") {
                        $groupElement.SetAttribute("targetFramework",  $groupName);
                    }

                    foreach($reference in $value.references) {
                        $referenceElement = $nuspec.CreateElement("reference", $xmlns);
                        $referenceElement.SetAttribute("file", $reference)
                        $groupElement.AppendChild($referenceElement) | Out-Null
                    }

                } 
            }
         
            $metadataElement.AppendChild($referencesElement) | Out-Null;
         }
         
 
        if($propertyName -eq "contentFiles") {
            $contentFilesElement = $nuspec.CreateElement("contentFiles", $xmlns);
         
            foreach($file in $json.contentFiles) {
                $filesElement = $nuspec.CreateElement("files", $xmlns);
                if($file.include -ne $null) {
                    $filesElement.SetAttribute("include", $xmlns, $file.include);
                }
                
                if($file.exclude -ne $null) {
                    $filesElement.SetAttribute("exclude", $xmlns, $file.exclude);
                }
                
                if($file.buildAction -ne $null) {
                    $filesElement.SetAttribute("buildAction", $xmlns, $file.buildAction);
                }
                
                if($file.copyToOutput -ne $null) {
                    $filesElement.SetAttribute("copyToOutput", $xmlns, $file.copyToOutput);
                }
                
                if($file.flatten -ne $null) {
                    $filesElement.SetAttribute("flatten", $xmlns, $file.flatten);
                }
                
                $contentFilesElement.AppendChild($filesElement) | Out-Null
            }

            $metadataElement.AppendChild($contentFilesElement) | Out-Null
        }
     }
     
    
     
     if($config.files -ne $null) {
         $filesElement = $nuspec.CreateElement("files", $xmlns);
         foreach($file in $config.files) {
             $fileElement = $nuspec.CreateElement("file", $xmlns);
             if($file.src -ne $null) {
                 $fileElement.SetAttribute("src", $file.src);
             }
             
             if($file.target -ne $null) {
                  $fileElement.SetAttribute("target",  $file.target);
             }
             
             if($file.exclude -ne $null) {
                  $fileElement.SetAttribute("exclude",  $file.exclude);
             }
             
             $filesElement.AppendChild( $fileElement) | Out-Null;
         }
         if($filesElement.ChildNodes.Count -gt 0){
              $nuspec.package.AppendChild($filesElement) | Out-Null;
         }
        
     }

    
      
     return $nuspec
}