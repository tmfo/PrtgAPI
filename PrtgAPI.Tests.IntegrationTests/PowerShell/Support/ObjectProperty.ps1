﻿. $PSScriptRoot\Init.ps1

function Describe($name, $script) {

    Pester\Describe $name {
        BeforeAll {
            Startup

            LogTest "Running unsafe test '$name'"
        }
        AfterAll { Shutdown }

        $object = $null

        function GetValue($property, $expected)
        {
            LogTestDetail "Processing property $property"

            $object | Assert-True -Message "Object was not initialized"
            $expected | Should Not BeNullOrEmpty

            $initialSettings = $object | Get-ObjectProperty

            $initialSettings.$property | Should Be $expected
        }

        function SetValue($property, $value)
        {
            LogTestDetail "Processing property $property"

            $object | Assert-True -Message "Object was not initialized"

            $initialSettings = $object | Get-ObjectProperty

            $object | Set-ObjectProperty $property $value

            $newSettings = $object | Get-ObjectProperty

            $newValue = $newSettings.$property
            $originalValue = $initialSettings.$property

            if($newValue.GetType().IsArray)
            {
                $newValue = [string]::Join("`n", $newValue)

                if($null -ne $originalValue)
                {
                    $originalValue = [string]::Join("`n", $originalValue)
                }

                if($null -ne $value -and $value.GetType().IsArray)
                {
                    $value = [string]::Join("`n", $value)
                }
            }

            $newValue | Assert-NotEqual $originalValue -Message "Expected initial and new value to be different, but they were both '<actual>'"
            $newValue | Should Not BeNullOrEmpty

            $newValue | Should Be $value
        }

        function SetChild($property, $value, $dependentProperty, $dependentValue)
        {
            LogTestDetail "Processing property $property"

            $object | Assert-True -Message "    Object was not initialized"
            $dependentProperty | Assert-True -Message "    Dependent property was not specified"

            $initialSettings = $object | Get-ObjectProperty
            $initialValue = $initialSettings.$property
            $initialDependent = $initialSettings.$dependentProperty

            $object | Set-ObjectProperty $property $value

            $newSettings = $object | Get-ObjectProperty
            $newValue = $newSettings.$property
            $newDependent = $newSettings.$dependentProperty

            $newValue | Assert-NotEqual $initialValue -Message "Expected initial and new value to be different, but they were both '<actual>'"
            $newDependent | Assert-NotEqual $initialDependent -Message "Expected initial and new dependent to be different, but they were both '<actual>'"
            $newValue | Should Not BeNullOrEmpty

            $newValue | Assert-Equal $value
            $newDependent | Assert-Equal $dependentValue

            $object | Set-ObjectProperty $dependentProperty $initialDependent
        }

        function SetGrandChild($property, $value, $middleProperty, $middleValue, $topProperty, $topValue)
        {
            LogTestDetail "Processing property $property"

            $object | Assert-True -Message "    Object was not initialized"
            $middleProperty | Assert-True -Message "    Middle property was not specified"
            $topProperty | Assert-True -Message "    Top property was not specified"

            $initialSettings = $object | Get-ObjectProperty
            $initialValue = $initialSettings.$property
            $initialMiddle = $initialSettings.$middleProperty
            $initialTop = $initialSettings.$topProperty

            $object | Set-ObjectProperty $property $value

            $newSettings = $object | Get-ObjectProperty
            $newValue = $newSettings.$property
            $newMiddle = $newSettings.$middleProperty
            $newTop = $newSettings.$topProperty

            $newValue | Assert-NotEqual $initialValue -Message "Expected initial and new value to be different, but they were both '<actual>'"
            $newMiddle | Assert-NotEqual $initialMiddle -Message "Expected initial and new middle to be different, but they were both '<actual>'"
            $newTop | Assert-NotEqual $initialTop -Message "Expected initial and new top to be different, but they were both '<actual>'"
            $newValue | Should Not BeNullOrEmpty

            $newValue | Assert-Equal $value
            $newMiddle | Assert-Equal $middleValue
            $newTop | Assert-Equal $topValue

            $object | Set-ObjectProperty $middleProperty $initialMiddle
            $object | Set-ObjectProperty $topProperty $initialTop
        }

        & $script
    }
}