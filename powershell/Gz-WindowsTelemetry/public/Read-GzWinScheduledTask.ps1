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
                    $lastRunDisplay = $lastRun.ToString()
                    $lastRun = ($lastRun.Ticks - 621355968000000000) / 10000;
                }

                IF($info.NextRunTime)
                {
                    $nextRun = $info.NextRunTime.ToUniversalTime()
                    $nextRunDisplay = $nextRun.ToString()
                    $nextRun = ($nextRun.Ticks - 621355968000000000) / 10000;
                }

                $lastResult = $info.LastTaskResult 
                $missedExecutions = $info.NumberOfMissedRuns
            }

            $now  = [DateTime]::UtcNow
            $epoc =  ($now.Ticks - 621355968000000000) / 10000;
            $tasks += [PsCustomObject]@{
                name = $st.TaskName 
                path = $st.TaskPath 
                status = $st.State 
                lastResult = $lastResult
                missedRuns = $missedExecutions
                lastRunAt = $lastRun
                lastRunAtDisplay = $lastRunDisplay
                nextRunAt = $nextRun
                nextRunDisplay = $nextRunDisplay
                rowCreatedAt = $epoch 
                rowUpdatedAt = $epoch
                rowRemovedAt = $null 
                rowCreatedAtDisplay = $now.ToString()
                rowUpdatedAtDisplay = $now.ToString()
                rowRemovedAtDisplay = $null
            }
        }

        return $tasks
    }
}