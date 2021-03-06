﻿. $PSScriptRoot\Support\Standalone.ps1

Describe "Start-AutoDiscovery" -Tag @("PowerShell", "UnitTest") {
    SetActionResponse

    It "pipes from a device" {
        $device = Run Device { Get-Device }

        $device | Start-AutoDiscovery
    }

    It "executes with -WhatIf" {
        $device = Run Device { Get-Device }

        $device | Start-AutoDiscovery -WhatIf
    }
}