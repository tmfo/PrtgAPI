﻿using System.Management.Automation;
using PrtgAPI.Objects.Shared;
using PrtgAPI.PowerShell.Base;

namespace PrtgAPI.PowerShell.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Moves a device or group within the PRTG Object Tree.</para>
    /// 
    /// <para type="description">The Move-Object cmdlet allows you to move a device or group to another group or probe within PRTG.
    /// Any device or group can be moved to any other group or probe, with the exception of special objects such as the "Probe Device"
    /// object under each probe, as well as the Root group (ID: 0).</para>
    /// 
    /// <example>
    ///     <code>Get-Device dc-1 | Move-Object 5678</code>
    ///     <para>Move all devices named dc-1 to under the object with ID 5678</para>
    /// </example>
    /// 
    /// <para type="link">Get-Device</para>
    /// <para type="link">Get-Group</para>
    /// </summary>
    [Cmdlet(VerbsCommon.Move, "Object", SupportsShouldProcess = true)]
    public class MoveObject : PrtgOperationCmdlet
    {
        /// <summary>
        /// <para type="description">The device to move to another group or probe.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Device")]
        public Device Device { get; set; }

        /// <summary>
        /// <para type="description">The group to move to another group or probe.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Group")]
        public Group Group { get; set; }

        /// <summary>
        /// <para type="description">The group or probe to move the object to. This cannot be the Root PRTG Group.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public int DestinationId { get; set; }

        /// <summary>
        /// Performs record-by-record processing functionality for the cmdlet.
        /// </summary>
        protected override void ProcessRecordEx()
        {
            switch (ParameterSetName)
            {
                case "Device":
                    ExecuteOperation(Device);
                    break;
                case "Group":
                    ExecuteOperation(Group);
                    break;
            }
        }

        private void ExecuteOperation(SensorOrDeviceOrGroupOrProbe obj)
        {
            if (ShouldProcess($"'{obj.Name}' (ID: {obj.Id})"))
            {
                ExecuteOperation(() => client.MoveObject(obj.Id, DestinationId), "Moving PRTG Objects", $"Moving {obj.BaseType.ToString().ToLower()} {obj.Name} (ID: {obj.Id}) to object ID {DestinationId}");
            }
        }
    }
}
