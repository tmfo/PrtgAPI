﻿using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using PrtgAPI.Attributes;
using DH = PrtgAPI.Objects.Deserialization.DeserializationHelpers;

namespace PrtgAPI.Objects.Shared
{
    /// <summary>
    /// <para type="description">Properties that apply to Sensors, Devices, Groups and Probes.</para>
    /// </summary>
    public class SensorOrDeviceOrGroupOrProbe : SensorOrDeviceOrGroupOrProbeOrLogOrTicket
    {
        // ################################## Sensors, Devices, Groups, Probes, Reports ##################################
        // There is a copy in both SensorOrDeviceOrGroupOrProbe and Report

        private string schedule;

        /// <summary>
        /// Monitoring schedule of this object. If this object is a report, this property displays the report generation schedule. If this object does not have a schedule, this value is null.
        /// </summary>
        [XmlElement("schedule")]
        [PropertyParameter(nameof(Property.Schedule))]
        public string Schedule
        {
            get { return schedule; }
            set { schedule = value == string.Empty ? null : value; }
        }

        // ################################## All Tree Objects ##################################

        /// <summary>
        /// Base type of this object ("sensor", "device", etc)
        /// </summary>
        [XmlElement("basetype")]
        [PropertyParameter(nameof(Property.BaseType))]
        public BaseType BaseType { get; set; }

        /// <summary>
        /// URL of this object.
        /// </summary>
        [XmlElement("baselink")]
        [PropertyParameter(nameof(Property.Url))]
        public string Url { get; set; }

        /// <summary>
        /// ID of this object's parent.
        /// </summary>
        [XmlElement("parentid")]
        [PropertyParameter(nameof(Property.ParentId))]
        public int ParentId { get; set; }

        // ################################## Sensors, Devices, Groups, Probes ##################################

        /// <summary>
        /// Number of each notification trigger type defined on this object, as well as whether this object inherits any triggers from its parent object.
        /// </summary>
        [PropertyParameter(nameof(Property.NotificationTypes))]
        public NotificationTypes NotificationTypes => notificationTypes == null ? new NotificationTypes(string.Empty) : new NotificationTypes(notificationTypes); //todo: add custom handling for this

        /// <summary>
        /// Raw notification types value of this object. This field is for internal use only.
        /// </summary>
        [XmlElement("notifiesx")]
        protected string notificationTypes { get; set; }

        /// <summary>
        /// Scanning interval for this sensor or default scanning interval for sensors under this object.
        /// </summary>
        [PropertyParameter(nameof(Property.Interval))]
        public TimeSpan Interval //todo: add custom handling for this
        {
            get
            {
                //Certain objects (like devices) do not report the intervals that have been defined when interval inheritance has been disabled.
                //As a workaround, when we can extract the value from their intervalx attributes instead.
                //If this statement is true, we've confirmed we need to make a last ditch effort to return a value.
                //Usually however, this expression will return false.
                if (interval == null)
                {
                    if (IntervalInherited == false)
                        //If IntervalInherited is false, _RawIntervalInherited should just contain a number.
                        return DH.ConvertPrtgTimeSpan(Convert.ToDouble(intervalInherited));
                    else //
                    {
                        var num = Regex.Replace(intervalInherited, "(.+\\()(.+)(\\))", "$2");

                        return DH.ConvertPrtgTimeSpan(Convert.ToDouble(num));
                    }
                }

                return DH.ConvertPrtgTimeSpan(interval.Value);
            }
        }

        /// <summary>
        /// Raw interval value of this object. This field is for internal use only.
        /// </summary>
        [XmlElement("interval_raw")]
        protected double? interval { get; set; }

        /// <summary>
        /// Whether this object's Interval is inherited from its parent object.
        /// </summary>
        [PropertyParameter(nameof(Property.IntervalInherited))]
        public bool IntervalInherited => intervalInherited?.Contains("Inherited") ?? false; //todo: add custom handling for this

        /// <summary>
        /// Raw inherited interval value of this object. This field is for internal use only.
        /// </summary>
        [XmlElement("intervalx")]
        protected string intervalInherited { get; set; }

        /// <summary>
        /// An <see cref="Access"/> value specifying the access rights of the API Request User on the specified object.
        /// </summary>
        [XmlElement("access_raw")]
        [PropertyParameter(nameof(Property.Access))]
        public Access Access { get; set; }

        /// <summary>
        /// Name of the object the monitoring of this object is dependent on. If dependency is on the parent object, value of DependencyName will be "Parent".
        /// </summary>
        [XmlElement("dependency")]
        [PropertyParameter(nameof(Property.Dependency))]
        public string Dependency { get; set; }

        /// <summary>
        /// Position of this object within its parent object.
        /// </summary>

        [PropertyParameter(nameof(Property.Position))]
        public int Position => position / 10;

        /// <summary>
        /// Raw position value of this object. This field is for internal use only.
        /// </summary>
        [XmlElement("position")]
        protected int position { get; set; }

        /// <summary>
        /// <see cref="PrtgAPI.Status"/> indicating this object's monitoring state.
        /// </summary>
        [XmlElement("status_raw")]
        [PropertyParameter(nameof(Property.Status))]
        public Status Status { get; set; }

        private string comments;

        /// <summary>
        /// Comments present on this object.
        /// </summary>
        [XmlElement("comments")]
        [PropertyParameter(nameof(Property.Comments))]
        public string Comments
        {
            get { return comments; }
            set { comments = value?.Trim(); }
        }
    }
}
