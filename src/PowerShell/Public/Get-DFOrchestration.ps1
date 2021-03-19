function Get-DFOrchestration
{
    [CmdletBinding()]
    param
    (
        # TaskHub instance
        [Parameter(Mandatory)]
        [DurableFunctionsCLI.Core.Models.TaskHub]
        $TaskHub,

        # Retrieve all orchestrations since this date (defaults to 1 hour ago)
        [Parameter()]
        [DateTime]
        $StartTime = (Get-Date).AddHours(-1),

        # Retrieve all orchestrations before this date (defaults to now)
        [Parameter()]
        [DateTime]
        $EndTime = (Get-Date)
    )
    
    begin {}
    
    process
    {
        $orchestrationFinder = [DurableFunctionsCLI.Core.Discovery.OrchestrationFinder]::new($TaskHub)
        $orchestrations = $orchestrationFinder.GetOrchestrations($StartTime, $EndTime)

        foreach ($orch in $orchestrations)
        {
            Add-JsonDeserialisedMember -InputObject $Orch -MemberName 'Input'
            Add-JsonDeserialisedMember -InputObject $Orch -MemberName 'Output'
        }

        $orchestrations
    }
    
    end {}
}