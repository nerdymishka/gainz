function Read-GzWinService() {
    [CmdletBinding()]
    Param(

    )
<#
.SYNOPSIS
    Reads a list of services.    

.DESCRIPTION
    Reads a list of services by name with the startupType,
    serviceType, status, and list of requiredServices.

.EXAMPLE
    PS C:\> $svcs = Read-GzWinService
   
.INPUTS
    None
    
.OUTPUTS
    An array of PsCustomObjects with the following properties:
        name = [string] name
        displayName = [string] titlized name
        requiredServices = [array[string]] an array of names for services this one depends on
        startupType = [enum|string]
        serviceType = [enum|string]
        status = string
        createdAt = [long] epoch  
        updatedAt = [long] epoch
        createdAtDisplay = [string] datetime in current users' dt format
        updatedAtDisplay = [string] datetime in current users' dt format

#>
    
    PROCESS {
        $svcs = Get-Service 
        $services = @()
        foreach($svc in $svcs) {
            $requiredSvc = $null;
            if($svc.ServicesDependOn.Length -gt 0) {
                $requiredSvc = @();
                foreach($p in $svc.ServicesDependOn) {
                    $requiredSvc.Add($p.Name);
                }
            }

            $now  = [DateTime]::UtcNow
            $epoc =  ($now.Ticks - 621355968000000000) / 10000;
            $services += [PsCustomObject]@{
                name = $svc.Name 
                displayName = $svc.DisplayName 
                requiredServices = $requiredSvc
                startupType = $svc.StartupType 
                serviceType = $svc.ServiceType
                status = $svc.ServiceType 
                rowCreatedAt = $epoch 
                rowUpdatedAt = $epoch
                rowRemovedAt = $null 
                rowCreatedAtDisplay = $now.ToString()
                rowUpdatedAtDisplay = $now.ToString()
                rowRemovedAtDisplay = $null
            }
        }

        return $services 
    }
}