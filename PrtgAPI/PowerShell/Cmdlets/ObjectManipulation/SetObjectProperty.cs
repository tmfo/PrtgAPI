﻿using System.Management.Automation;
using System.Reflection;
using PrtgAPI.Attributes;
using PrtgAPI.Helpers;
using PrtgAPI.Objects.Shared;
using PrtgAPI.Parameters;
using PrtgAPI.PowerShell.Base;

namespace PrtgAPI.PowerShell.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Modifies the value of an object property.</para>
    /// <para type="description">The Set-ObjectProperty cmdlet modifies properties and settings of PRTG Sensors, Devices, Groups and Probes.
    /// All supported properties that can be modified are typesafe, using the type of the property on the Settings object returned from
    /// Get-ObjectProperty.</para>
    /// <para type="description">When a value is specified, Set-ObjectProperty will attempt to parse the value into its expected type. If the
    /// type cannot be parsed, an exception will be thrown indicating the type of the object specified and the type of value that was expected.
    /// In the case of enums, Set-ObjectProperty will list all valid values of the target type so that you may know how exactly to interface with the
    /// specified property.</para>
    /// <para type="description">In the event a property is specified that has a dependency on another property being set (such as Interval requiring
    /// InheritInterval be $false, DBPort require DBPortMode be manual, etc) Set-ObjectProperty will automatically assign the required values such that
    /// the original property may be correctly enabled.</para>
    /// <para type="description">For properties that are not currently supported by PrtgAPI, these settings can still be interfaced with via
    /// the -RawProperty and -RawValue parameters. When -Raw* parameters are specified, PrtgAPI will not perform any validation on the specified values.
    /// The -RawProperty parameter must contain the raw property name, as typically found on the "name" attribute of  the &lt;input/&gt; tag
    /// under the object's Settings page in the PRTG UI. All properties that end in an underscore should include this in their names. All properties
    /// typically end in an underscore, with the exception of section inheritance properties (Inherit Windows Credentials, etc)</para>
    /// <para type="description">For a list of settings currently supported by Set-ObjectProperty, see Get-Help about_ObjectSettings and about_SensorSettings.</para>
    /// 
    /// <example>
    ///     <code>C:\> Get-Sensor -Id 1001 | Set-ObjectProperty Interval "00:00:30"</code>
    ///     <para>C:\> Get-Sensor -Id 1001 | Set-ObjectProperty Interval ([TimeSpan]"00:00:30")</para>
    ///     <para>C:\> Get-Sensor -Id 1001 | Set-ObjectProperty Interval ThirtySeconds</para>
    ///     <para>Set the Scanning Interval of the sensor with ID 1001 to 30 seconds. Type "ScanningInterval" expected by property Interval
    ///     will attempt to coerce strings, TimeSpans and enums into a ScanningInterval object. If the cmdlet succeeds, InheritInterval will also be set to false.</para>
    ///     <para/>
    /// </example>
    /// <example>
    ///     <code>C:\> Get-Sensor -Id | 1001 | Set-ObjectProperty -RawProperty name_ -RawValue newName</code>
    ///     <para>Set raw property "name" to value "newName"</para>
    /// </example>
    /// 
    /// <para type="link">about_ObjectSettings</para>
    /// <para type="link">about_SensorSettings</para>
    /// <para type="link">Get-ObjectProperty</para>
    /// <para type="link">Get-Sensor</para>
    /// <para type="link">Get-Device</para>
    /// <para type="link">Get-Probe</para>
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "ObjectProperty", SupportsShouldProcess = true)]
    public class SetObjectProperty : PrtgOperationCmdlet
    {
        /// <summary>
        /// <para type="description">The object to modify the properties of.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public SensorOrDeviceOrGroupOrProbe Object { get; set; }

        /// <summary>
        /// <para type="description">The property to modify.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "Default")]
        public ObjectProperty Property { get; set; }

        /// <summary>
        /// <para type="description">The value to set for the specified property.</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "Default")]
        public object Value { get; set; }

        /// <summary>
        /// <para type="description">The raw name of the property to modify. This can be typically discovered by inspecting the "name" attribute of the properties' &lt;input/&gt; tag on the Settings page of PRTG.</para>
        /// This value typically ends in an underscore.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Unsafe")]
        public string RawProperty { get; set; }

        /// <summary>
        /// <para type="description">The value to set the object's property to. For radio buttons and dropdown lists, this is the integer found in the 'value' attribute.<para/>
        /// WARNING: If an invalid value is set for a property, minor corruption may occur.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Unsafe")]
        public string RawValue { get; set; }

        /// <summary>
        /// <para type="description">Sets an unsafe object property without prompting for confirmation. WARNING: Setting an invalid value for a property can cause minor corruption. Only use if you know what you are doing.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Unsafe")]
        public SwitchParameter Force { get; set; }

        /// <summary>
        /// Performs record-by-record processing functionality for the cmdlet.
        /// </summary>
        protected override void ProcessRecordEx()
        {
            if (ParameterSetName == "Default")
            {
                //Value is not required, but is required in that we need to explicitly say null
                if (!MyInvocation.BoundParameters.ContainsKey("Value"))
                    throw new ParameterBindingException("Value parameter is mandatory, however a value was not specified. If Value should be empty, specify $null");

                ParseValue();

                if (ShouldProcess($"{Object.Name} (ID: {Object.Id})", $"Set-ObjectProperty {Property} = '{Value}'"))
                    ExecuteOperation(() => client.SetObjectProperty(Object.Id, Property, Value), "Modify PRTG Object Settings", $"Setting object '{Object.Name}' (ID: {Object.Id}) setting '{Property}' to '{Value}'");
            }
            else
            {
                if (Force || ShouldContinue($"Are you sure you want to set raw object property '{RawProperty}' to value '{RawValue}'? This may cause minor corruption if the specified value is not valid for the target property. Only proceed if you know what you are doing.", "WARNING!"))
                {
                    if (ShouldProcess($"{Object.Name} (ID: {Object.Id})", $"Set-ObjectProperty {Property} = '{Value}'"))
                        ExecuteOperation(() => client.SetObjectPropertyRaw(Object.Id, RawProperty, RawValue), "Modify PRTG Object Settings", $"Setting object '{Object.Name}' (ID: {Object.Id}) setting '{RawProperty}' to '{RawValue}'");
                }
            }
        }

        private void ParseValue()
        {
            var prop = BaseSetObjectPropertyParameters<ObjectProperty>.GetPropertyInfoViaTypeLookup(Property);
            Value = ParseValueIfRequired(prop, Value);
        }
    }
}
