function Add-OrchestrationObjectMembers
{
    [CmdletBinding()]
    param
    (
        # The orchestration to add the object members to
        [Parameter(Mandatory)]
        [DurableFunctionsCLI.Core.Models.Orchestration]
        $Orchestration
    )
    
    begin {}
    
    process
    {
        if (Test-Json -Json $Orchestration.Input -ErrorAction SilentlyContinue)
        {
            $inputValue = { $this.Input | ConvertFrom-Json }
        }
        else
        {
            $inputValue = { $this.Input }
        }

        $Orchestration | Add-Member -MemberType ScriptProperty -Name 'InputObject' -Value $inputValue

        if (Test-Json -Json $Orchestration.Output -ErrorAction SilentlyContinue)
        {
            $outputValue = { $this.Output | ConvertFrom-Json }
        }
        else
        {
            $outputValue = { $this.Output }
        }

        $Orchestration | Add-Member -MemberType ScriptProperty -Name 'OutputObject' -Value $outputValue
    }
    
    end {}
}