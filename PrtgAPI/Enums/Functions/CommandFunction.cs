﻿using System.ComponentModel;
using PrtgAPI.Attributes;

namespace PrtgAPI
{
    enum CommandFunction
    {
        [Description("rename.htm")]
        Rename,

        [Description("setpriority.htm")]
        SetPriority,

        [Description("setobjectproperty.htm")]
        SetObjectProperty,

        [Description("pause.htm")]
        Pause,

        [Description("pauseobjectfor.htm")]
        PauseObjectFor,

        [Description("simulate.htm")]
        Simulate,

        [Description("acknowledgealarm.htm")]
        AcknowledgeAlarm,

        [Description("scannow.htm")]
        ScanNow,

        [Description("discovernow.htm")]
        DiscoverNow,

        [Description("setposition.htm")]
        SetPosition,

        [Description("reportaddsensor.htm")]
        ReportAddSensor,

        [Description("notificationtest.htm")]
        NotificationTest,

        [Description("deleteobject.htm")]
        DeleteObject,

        [Description("duplicateobject.htm")]
        DuplicateObject,

        [Description("setlonlat.htm")]
        SetLonLat,

        [Undocumented]
        [Description("moveobjectnow.htm")]
        MoveObjectNow,

        [Description("sortsubobjects.htm")]
        SortSubObjects,

        [Undocumented]
        [Description("addsensor5.htm")]
        AddSensor5
    }
}
