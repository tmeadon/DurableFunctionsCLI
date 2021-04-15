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
        $rgExists = Test-ResourceGroupExists -ResourceGroupName $ResourceGroupName

        if (-not $rgExists) { throw "Resource Group $ResourceGroupName can not be found in subscription $subscriptionId" }

        # build a task hub finder instance
        $taskHubFinderParams = @{
            SubscriptionId    = $subscriptionId
            Token             = $token
            ResourceGroupName = $ResourceGroupName
        }
        if ($StorageAccountName) { $taskHubFinderParams['StorageAccountName'] = $StorageAccountName }
        $taskHubFinder = Build-TaskHubFinder @taskHubFinderParams

        # find all task hubs
        try
        {
            Find-TaskHubs -TaskHubFinder $taskHubFinder
        }
        catch
        {
            if ($_.Exception.InnerException.InnerException -is [DurableFunctionsCLI.Core.Exceptions.StorageAccountNotFoundException])
            {
                throw "Could not retrieve storage account(s), please check $ResourceGroupName is a valid resource group in subscription $subscriptionId"
            }
            if ($_.Exception.InnerException.InnerException -is [DurableFunctionsCLI.Core.Exceptions.StorageApiThrottledException])
            {
                throw "Could not retrieve storage account(s) because storage API requests are being throttled.  Please try again later or try adding the -StorageAccountName parameter for cheaper requests"
            }

            throw "Unknown error occurred retrieving task hubs: $($_.Exception)"
        }
    }
    
    end {}
}