function Read-GzWinScheduledTask() {
    [CmdletBinding()]
    Param(

    )
<#

#>
    
    PROCESS {
        $scheduledTasks = Get-ScheduledTask
        $tasks = @()
        foreach($st in $scheduledTasks) {
           
            $info = Get-ScheduledTaskInfo -TaskName $st.TaskName -EA SilentlyContinue 
            $lastRun = $null 
            $lastRunDisplay = $null;
            $nextRun = $null;
            $nextRunDisplay = $null;
            $lastResult = $null;
            $missedExecutions = $null

            if($info)
            {
                if($info.LastRunTime)
                {
                    $lastRun = $info.LastRunTime.ToUniversalTime()
                }

                IF($info.NextRunTime)
                {
                    $nextRun = $info.NextRunTime.ToUniversalTime()
                }

                $lastResult = $info.LastTaskResult 
                $missedExecutions = $info.NumberOfMissedRuns
            }

            $now  = [DateTime]::UtcNow
          
            $tasks += [PsCustomObject]@{
                name = $st.TaskName 
                path = $st.TaskPath 
                status = $st.State 
                lastResult = $lastResult
                missedRuns = $missedExecutions
                lastRunAt = $lastRun
                nextRunAt = $nextRun
                createdAt = $now 
            }
        }

        return $tasks
    }
}