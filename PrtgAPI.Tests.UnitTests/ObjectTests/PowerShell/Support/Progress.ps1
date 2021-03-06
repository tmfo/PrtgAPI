﻿. $PSScriptRoot\UnitTest.ps1

function Clear-Progress {

    $val = [PrtgAPI.Tests.UnitTests.InfrastructureTests.Support.Progress.ProgressQueue]::ProgressSnapshots

    if($val -ne $null)
    {
        $val.Clear()
    }    
}

function Describe($name, $script)
{
    Pester\Describe $name {

        BeforeAll {    InitializeUnitTestModules }
        AfterEach {
            [PrtgAPI.Tests.UnitTests.InfrastructureTests.Support.Progress.ProgressQueue]::RecordQueue.Clear()
            Clear-Progress
        }

        & $script
    }
}

function Get-Progress {
    return [PrtgAPI.Tests.UnitTests.InfrastructureTests.Support.Progress.ProgressQueue]::Dequeue()
}

function Validate($list)    {

    foreach($progress in $list)
    {
        Get-Progress | Should Be $progress
    }

    try
    {
        { $result = Get-Progress; throw "`n`nProgress Queue contains more records than expected. Next record is:`n`n$result`n`n" } | Should Throw "Queue empty"
    }
    catch [exception]
    {
        Clear-Progress
        throw
    }
}

function InitializeClient {
    [PrtgAPI.Tests.UnitTests.InfrastructureTests.Support.MockProgressWriter]::Bind()

    SetMultiTypeResponse

    Enable-PrtgProgress
}

function RunCustomCount($hashtable, $action)
{
    $dictionary = GetCustomCountDictionary $hashtable

    $oldClient = Get-PrtgClient

    $newClient = [PrtgAPI.Tests.UnitTests.ObjectTests.BaseTest]::Initialize_Client((New-Object PrtgAPI.Tests.UnitTests.ObjectTests.TestResponses.MultiTypeResponse -ArgumentList $dictionary))

    try
    {
        SetPrtgClient $newClient

        & $action
    }
    catch
    {
        throw
    }
    finally
    {
        SetPrtgClient $oldClient
    }
}