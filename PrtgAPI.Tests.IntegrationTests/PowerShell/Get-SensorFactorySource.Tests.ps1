﻿. $PSScriptRoot\Support\IntegrationTestSafe.ps1

Describe "Get-SensorFactorySource_IT" {
    It "retrieves all sensors" {
        $sensors = Get-Sensor -Id (Settings SensorFactory) | Get-SensorFactorySource

        $sensors.Count | Should Be 1

        $sensors.GetType().Name | Should Be "Sensor"
    }

    It "retrieves all channels" {
        $channels = Get-Sensor -Id (Settings SensorFactory) | Get-SensorFactorySource -Channels

        $channels.Count | Should Be 1

        $channels.GetType().Name | Should Be "Channel"
    }
}