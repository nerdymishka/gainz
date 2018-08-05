

function Read-ModuleHelp() {
    [CmdletBinding()]
    Param(
        
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
        $help = (Get-Command -module $module) | Get-Help -Full | Where-Object {! $_.Name.EndsWith('.ps1') }

        foreach ($cmdlet in $help){
            $info = (Get-Command $cmdlet.Name)
        
           
            $alias = (Get-Alias -Definition $info.Name -ErrorAction SilentlyContinue)
            if($alias) {
                $info | Add-Member -MemberType NoteProperty -Name Alias -Value $alias
            }
        
            # Parse the related links and assign them to a links hashtable.
            if(($info.RelatedLinks | Out-String).Trim().Length) {
                $links = $info.RelatedLinks.NavigationLink | % {
                    if($_.uri) { 
                        return @{
                            Name = $_.uri 
                            Link = $_.uri
                            Target='_blank'
                        };
                    }
                    if($_.linkText){ 
                       return @{
                           Name = $_.linkText; 
                           Link = "#$($_.linkText)"; 
                           CssClass = 'posh-link'; 
                           Target='_top'
                        } 
                    }
                    return $null
                }
                $info | Add-Member -MemberType NoteProperty -Name Links -Value $links
            }
        
            # Add parameter aliases to the object.
            foreach($parameter in $info.Parameters.Parameter ){
                $paramAliases = ($info.Parameters.Values | Where-Object { $_.Name -like $parameter.name } | Select-Object Aliases).Aliases
                if($paramAliases){
                    $parameter | Add-Member -MemberType NoteProperty -Name Aliases -Value "$($paramAliases -join ', ')" -Force
                }
            }
        }
        return $help 
    }
}

