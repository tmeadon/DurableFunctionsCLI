# DurableFunctionsCLI

This repository contains the beginnings of an alternative CLI for Azure Durable Functions in the form of a PowerShell module.

## Module Installation

Prerequisites:
- [Azure PowerShell](https://docs.microsoft.com/en-us/powershell/azure/install-az-ps?view=azps-6.4.0)

```powershell
Install-Module PSDurableFunctions
```

## Commands

- `Get-DFTaskHubs` - this command lists all Durable Function task hubs in the given Resource Group (specified using the `-ResourceGroupName` parameter).  It can be narrowed down using the `-StorageAccountName` parameter
- `Get-DFOrchestration` - this command lists all orchestration instances in the given TaskHub (specified using the `-TaskHub` parameter) in the last hour.  The time range can be modified using the `-StartTime` and `-EndTime` parameters
- `Expand-DfOrchestration` - this command takes a specific orchestration instance in a specific TaskHub (specific using the `-ExecutionId` and `-TaskHub` parameters respectively) and returns details about its constituent steps (start/end times, inputs/outputs etc)