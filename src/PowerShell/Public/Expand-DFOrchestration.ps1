function Expand-DFOrchestration
{
    [CmdletBinding()]
    param
    (    
        # Task hub containing the orchestration
        [Parameter(Mandatory)]
        [DurableFunctionsCLI.Core.Models.TaskHub]
        $TaskHub,

        # Execution ID for the orchestration
        [Parameter(Mandatory, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [string]
        $ExecutionId
    )
    
    begin {}
    
    process
    {
        $expander = [DurableFunctionsCLI.Core.Discovery.OrchestrationExpander]::new($TaskHub, $ExecutionId)
        $events = $expander.GetOrchestrationEvents()

        foreach ($item in $events)
        {
            Add-JsonDeserialisedMember -InputObject $item -MemberName 'Result'
        }

        $events
    }
    
    end {}
}