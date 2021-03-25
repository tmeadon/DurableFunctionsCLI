using namespace DurableFunctionsCLI.Core.Discovery

function Get-DFTaskHubs
{
    [CmdletBinding()]
    param
    (
        # The resource group to search in
        [Parameter(Mandatory)]
        [string]
        $ResourceGroupName,

        # The specific storage account to search in
        [Parameter()]
        [string]
        $StorageAccountName
    )
    
    begin {}
    
    process
    {
        $token = Get-AzureAccessToken
        $subscriptionId = Get-ConnectedSubscriptionId

        # build a task hub finder instance
        $taskHubFinderParams = @{
            SubscriptionId    = $subscriptionId
            Token             = $token
            ResourceGroupName = $ResourceGroupName
        }
        if ($StorageAccountName) { $taskHubFinderParams['StorageAccountName'] = $StorageAccountName }
        $taskHubFinder = Build-TaskHubFinder @taskHubFinderParams

        # find all task hubs
        Find-TaskHubs -TaskHubFinder $taskHubFinder
    }
    
    end {}
}