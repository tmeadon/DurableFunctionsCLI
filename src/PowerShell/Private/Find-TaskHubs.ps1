function Find-TaskHubs
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory)]
        [DurableFunctionsCLI.Core.Discovery.TaskHubFinder]
        $TaskHubFinder
    )
    
    begin {}
    
    process
    {
        $findTask = $TaskHubFinder.FindAllAsync()
        $findTask.Wait()
        $result = $findTask.GetAwaiter().GetResult()
        
        if ($findTask.IsCompletedSuccessfully)
        {
            $result
        }
        else
        {
            throw $result
        }
    }
    
    end {}
}