

# https://devblogs.microsoft.com/scripting/reusing-powershell-registry-time-stamp-code/
function Add-RegKeyMember {
    [CmdletBinding()]
    Param(

        [Parameter(Mandatory, ParameterSetName="ByKey", Position=0, ValueFromPipeline)]
        [Microsoft.Win32.RegistryKey] $RegistryKey,

        [Parameter(Mandatory, ParameterSetName="ByPath", Position=0)]
        [string] $Path
    )



    Begin {

        # Define the namespace (string array creates nested namespace):
    
        $namespace = "Gz"
    
     
    
        # Make sure type is loaded (this will only get loaded on first run):
    
Add-Type @"
    
            using System;
    
            using System.Text;
    
            using System.Runtime.InteropServices;
    
     
    
            $($namespace | ForEach-Object {
    
                "namespace $_ {"
    
            })
    
     
    
                public class advapi32 {
    
                    [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
    
                    public static extern Int32 RegQueryInfoKey(
    
                        Microsoft.Win32.SafeHandles.SafeRegistryHandle hKey,
    
                        StringBuilder lpClass,
    
                        [In, Out] ref UInt32 lpcbClass,
    
                        UInt32 lpReserved,
    
                        out UInt32 lpcSubKeys,
    
                        out UInt32 lpcbMaxSubKeyLen,
    
                        out UInt32 lpcbMaxClassLen,
    
                        out UInt32 lpcValues,
    
                        out UInt32 lpcbMaxValueNameLen,
    
                        out UInt32 lpcbMaxValueLen,
    
                        out UInt32 lpcbSecurityDescriptor,
    
                        out System.Runtime.InteropServices.ComTypes.FILETIME lpftLastWriteTime
    
                    );
    
                }
    
            $($namespace | ForEach-Object { "}" })
    
"@
    
        $RegTools = ("{0}.advapi32" -f ($Namespace -join ".")) -as [type]
    }
    
   
    
    Process {
        switch ($PSCmdlet.ParameterSetName) {
            # Already have the key, no more work to be done 🙂
            "ByKey" {}
            "ByPath" {
                # We need a RegistryKey object (Get-Item should return that
                $Item = Get-Item -Path $Path -ErrorAction Stop
    
     
                # Make sure this is of type [Microsoft.Win32.RegistryKey]
                if ($Item -isnot [Microsoft.Win32.RegistryKey]) {
                    throw "'$Path' is not a path to a registry key!"
                }
    
                $RegistryKey = $Item
    
            }
    
        }
    
        $classLength = 255 # Buffer size (class name is rarely used, and when it is, I've never seen
        $className = New-Object System.Text.StringBuilder $classLength  # Will hold the class name
    
        $LastWriteTime = New-Object System.Runtime.InteropServices.ComTypes.FILETIME 
    
        $query = $RegTools::RegQueryInfoKey($RegistryKey.Handle,
            $ClassName,
            [ref] $ClassLength,
            $null,  # Reserved
            [ref] $null, # SubKeyCount
            [ref] $null, # MaxSubKeyNameLength
            [ref] $null, # MaxClassLength
            [ref] $null, # ValueCount
            [ref] $null, # MaxValueNameLength
            [ref] $null, # MaxValueValueLength
            [ref] $null, # SecurityDescriptorSize
            [ref] $LastWriteTime)
    
        switch ($query) {
    
            0 { # Success
                $lo = [System.BitConverter]::ToUInt32([System.BitConverter]::GetBytes($LastWriteTime.dwLowDateTime), 0)
                $high = [System.BitConverter]::ToUInt32([System.BitConverter]::GetBytes($LastWriteTime.dwHighDateTime), 0)
    
                $fileTimeInt64 = ([Int64] $high -shl 32) -bor $lo
                $modifiedAt = [datetime]::FromFileTime($fileTimeInt64)

                Write-Host $modifiedAt;
                
    
                $RegistryKey | Add-Member -NotePropertyMembers @{
                    LastWriteTime = $modifiedAt
                    ClassName = $ClassName.ToString()
                } -PassThru -Force
  
            }
    
            122  { # ERROR_INSUFFICIENT_BUFFER (0x7a)
    
                throw "Class name buffer too small"
    
            }
            default {
    
                throw "Unknown error encountered (error code $_)"
    
            }
    
        }
    
    }

}