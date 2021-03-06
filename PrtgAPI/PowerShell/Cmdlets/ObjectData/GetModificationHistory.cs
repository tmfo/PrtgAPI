﻿using System;
using System.Collections.Generic;
using System.Management.Automation;
using PrtgAPI.Objects.Shared;
using PrtgAPI.PowerShell.Base;

namespace PrtgAPI.PowerShell.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Retrieves the setting/state modification history of a PRTG Object.</para>
    /// 
    /// <para type="description">THe Get-ModificationHistory cmdlet retrieves all setting/state modifications of an object.
    /// The Get-ModificationHistory cmdlet corresponds with the "History" tab of objects in the PRTG UI.</para>
    /// 
    /// <example>
    ///     <code>C:\> Get-Sensor -Id 1001 | Get-ModificationHistory</code>
    ///     <para>Retrieve all modification events for the sensor with ID 1001</para>
    /// </example>
    /// 
    /// <para type="link">Get-Sensor</para>
    /// <para type="link">Get-Device</para>
    /// <para type="link">Get-Group</para>
    /// <para type="link">Get-Probe</para>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ModificationHistory")]
    public class GetModificationHistory : PrtgObjectCmdlet<ModificationEvent>
    {
        /// <summary>
        /// <para type="description">The object to retrieve historical data for.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Default", ValueFromPipeline = true)]
        public SensorOrDeviceOrGroupOrProbe Object { get; set; }

        /// <summary>
        /// <para type="description">The ID of the object to retrieve historical data for.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Manual")]
        public int Id { get; set; }

        /// <summary>
        /// Performs record-by-record processing functionality for the cmdlet.
        /// </summary>
        protected override IEnumerable<ModificationEvent> GetRecords()
        {
            if (ParameterSetName == "Default")
                Id = Object.Id;

            var results = client.GetModificationHistory(Id);

            return results;
        }
    }
}
