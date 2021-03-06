﻿using System.Xml.Linq;
using PrtgAPI.Tests.UnitTests.ObjectTests.TestItems;

namespace PrtgAPI.Tests.UnitTests.ObjectTests.TestResponses
{
    public class MessageResponse : BaseResponse<MessageItem>
    {
        internal MessageResponse(params MessageItem[] messages) : base("messages", messages)
        {
        }

        public override XElement GetItem(MessageItem item)
        {
            var xml = new XElement("item",
                new XElement("datetime", item.DateTime),
                new XElement("datetime_raw", item.DateTimeRaw),
                new XElement("parent", item.Parent),
                new XElement("status", item.Status),
                new XElement("status_raw", item.StatusRaw),
                new XElement("sensor", item.Sensor),
                new XElement("device", item.Device),
                new XElement("group", item.Group),
                new XElement("probe", item.Probe),
                new XElement("priority", item.Priority),
                new XElement("message", item.Message),
                new XElement("message_raw", item.MessageRaw),
                new XElement("type", item.Type),
                new XElement("type_raw", item.TypeRaw),
                new XElement("tags", item.Tags),
                new XElement("active", item.Active),
                new XElement("active_raw", item.ActiveRaw),
                new XElement("objid", item.ObjId),
                new XElement("name", item.Name)
            );

            return xml;
        }
    }
}
