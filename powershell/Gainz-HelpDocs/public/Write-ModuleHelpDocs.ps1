function Write-ModuleHelpDocs() {
    <#
    .SYNOPSIS
        Writes help docs for a given powershell module.
    .DESCRIPTION
        Writes help docs for a given powershell module in
        dfx flavored markdown.

    .PARAMETER Module
        Required. The target module for generating help docs.
    
    .PARAMETER Path
        Optional. The output directory. Aliases: -Dest,-Destination,-O
        
    .PARAMETER Do
        Optional. Do is an action that overrides the default behavior for
        `Write-CmdletDoc`. The default Do action is a pipeline of functions
        that are passed a model.  The Do action can be used to override 
        item templates for each part generating help docs.

    .PARAMETER PostProcess 
        Optional.  A script block with the parameters of for the 
        file that is generated and the associated PowerShell help
        meta data so that a generate file can be processed after
        it is generated.

    .PARAMETER PublishedAt
        Optional. Generates and inserts the published at date for
        all the generated help documents.

    .PARAMETER BaseUri
        Optional. Sets the base uri for the documents when the
        help doc module is bundled under a larger site.

    .EXAMPLE
        PS C:\> Write-Help -Module "PsReadline" -O "$Home/Desktop/PsReadline"

    #>
    [CmdletBinding()]
    Param(
        [Alias("Destination")]
        [Alias("Dest")]
        [Alias("O")]
        [Parameter(Position = 1)]
        [String] $Path,

        [ScriptBlock] $Do,
        
        [ScriptBlock] $PostProcess,

        [String] $PublishedAt,

        [String] $BaseUri 
    )

    DynamicParam {
        
        $parameterAttr = New-Object  System.Management.Automation.ParameterAttribute -Property @{
            Mandatory = $true 
            Position = 0
            ParameterSetName =  '__AllParameterSets'
        }

        $list = New-Object  System.Collections.ObjectModel.Collection[System.Attribute]
        $list.Add($parameterAttr)

        $modules =  Get-Module | Select-Object -ExpandProperty Name
        $validateSetAttr = New-Object System.Management.Automation.ValidateSetAttribute($modules)
        $list.Add($validateSetAttr)

        $moduleParam = New-Object  System.Management.Automation.RuntimeDefinedParameter('Module', [string],  $list)

        $parameters = New-Object  System.Management.Automation.RuntimeDefinedParameterDictionary
        $parameters.Add('Module', $moduleParam)

        return $parameters
    }

    Process {
        $module = $PsBoundParameters.module 
        
        if($module) {
            $mod = Get-Module $module 
            $author = $mod.Author
            $help = Read-ModuleHelp -Module $module
           
            if($help) {
                foreach ($cmdlet in $help) {
                   $name = $cmdlet.Name
                  
                  
                   $sb = New-Object System.Text.StringBuilder
                   $isUpper = $false
                   $last = $null; 
              
                   for($i = 0; $i -lt $name.Length; $i++) {
                       
                      $c = $name[$i]


                       if([Char]::IsUpper($c)) {
                           if($i -eq 0) {
                               $sb.Append($c.ToString().ToLower())  | Out-Null ;
                               continue;
                           }

                           $next = $name[$i + 1]
                           if([Char]::IsUpper($next)) {
                               $sb.Append($c.ToString().ToLower())  | Out-Null ;
                               $isUpper = $true;
                              
                               continue;
                           }

                           
                            if([Char]::IsLower($next)) {
                                $last = $name[$i-1];
                                if($last -ne "-") {
                                    $sb.Append("-") | Out-Null ;
                                }
                                
                                $sb.Append($c.ToString().ToLower())  | Out-Null ;
                                continue;
                            }
                           
                           
                           $sb.Append($c.ToString().ToLower())  | Out-Null ;
                           continue;
                       }
                       

                       $isUpper = $false;
                       $sb.Append($c) | Out-Null ;
                   }

                   $name = $sb.ToString()
                   $now = [DateTime]::UtcNow.ToShortDateString();
                   $cmdlet | Add-Member -MemberType NoteProperty -Name "UpdatedAt" -Value $now
                   $cmdlet | Add-Member -MemberType NoteProperty -Name "HyphenatedName" -Value $Name
                   $cmdlet | Add-Member -MemberType NoteProperty -Name Author -Value $author
                   #$cmdlet | Add-Member -MemberType NoteProperty -Name ModuleName -Value $module
                   $cmdlet | Add-Member -MemberType NoteProperty -Name PublishedAt -Value $PublishedAt
                   $cmdlet | Add-Member -MemberType NoteProperty -Name BaseUri -Value $BaseUri
                   $cmdlet | Add-Member -MemberType NoteProperty -Name Version -Value $mod.Version
                   $cmdlet | Add-Member -MemberType NoteProperty -Name Company -Value $mod.Company 
                   $cmdlet | Add-Member -MemberType NoteProperty -Name Tags -Value $mod.Tags 
                   $out = Write-CmdletDoc -Model $cmdlet -Do $Do
                   $out = $out.Trim()

                   if(!(Test-Path $Path)) {
                        New-Item $Path -ItemType Directory | Write-Debug
                   }

                   $out | Out-File "$Path/$name.md" -Encoding "UTF8"
                }

                if($PostProcess) {
                    & $PostProcess -Path $path -Help $help 
                }
            }
        } 
    }
}
Set-Alias -Name "Write-Help" -Value "Write-ModuleHelpDocs"