﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using PrtgAPI.Attributes;
using PrtgAPI.Helpers;

namespace PrtgAPI.Objects.Deserialization
{
    [ExcludeFromCodeCoverage]
    class XmlSerializer
    {
        private Type outerType;

        public XmlSerializer(Type type)
        {
            outerType = type;
        }

        public object Deserialize(XDocument doc)
        {
            return Deserialize(outerType, doc.Elements().First());
        }

        private object Deserialize(Type type, XElement elm)
        {
            var obj = Activator.CreateInstance(type);

            var mappings = GetMappings(type);

            foreach (var mapping in mappings)
            {
                Logger.Debug($"\nDeserialize property {mapping.Property.Name}: ");

                try
                {
                    switch (mapping.AttributeType)
                    {
                        case XmlAttributeType.Element:
                            ProcessXmlElement(obj, mapping, elm);
                            break;
                        case XmlAttributeType.Attribute:
                            ProcessXmlAttribute(obj, mapping, elm);
                            break;
                        case XmlAttributeType.Text:
                            ProcessXmlText(obj, mapping, elm);
                            break;
                        default:
                            throw new NotSupportedException(); //todo: add an appropriate failure message
                    }
                }
                catch (Exception)
                {
                    //throw new Exception("An error occurred while trying to deserialize property " + mapping.Property.Name);
                    throw;
                }
                
            }

            Logger.Debug("\n");

            return obj;
        }

        private List<XmlMapping> GetMappings(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var mappings = new List<XmlMapping>();

            foreach (var prop in properties)
            {
                if (FindXmlAttribute<XmlElementAttribute>(prop, mappings, type, a => a.ElementName, XmlAttributeType.Element))
                    continue;
                if (FindXmlAttribute<XmlAttributeAttribute>(prop, mappings, type, a => a.AttributeName, XmlAttributeType.Attribute))
                    continue;
                if (FindXmlAttribute<XmlTextAttribute>(prop, mappings, type, a => null, XmlAttributeType.Text))
                    continue;
            }

            return mappings;
        }

        private bool FindXmlAttribute<TAttribute>(PropertyInfo property, List<XmlMapping> mappings, Type type, Func<TAttribute, string> name, XmlAttributeType enumType) where TAttribute : Attribute
        {
            var attributes = property.GetCustomAttributes(typeof (TAttribute)) as IEnumerable<TAttribute>;

            if (attributes != null)
            {
                var list = attributes.ToList();

                if (list.Any())
                {
                    mappings.Add(new XmlMapping(type, property, list.Select(name).ToArray(), enumType));
                }
                else
                    return false;
                return true;
            }

            return false;
        }

        private void ProcessXmlElement(object obj, XmlMapping mapping, XElement elm)
        {
            var type = mapping.Property.PropertyType;
            
            if (typeof (IEnumerable).IsAssignableFrom(type) && type != typeof (string))
            {
                ProcessEnumerableXmlElement(obj, mapping, elm);
            }
            else
            {
                ProcessSingleXmlElement(obj, mapping, elm);
            }
        }

        private void ProcessEnumerableXmlElement(object obj, XmlMapping mapping, XElement elm)
        {
            var type = mapping.Property.PropertyType;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                ProcessListEnumerableXmlElement(obj, mapping, elm);
            }
            else
            {
                ProcessNonListEnumerableXmlElement(obj, mapping, elm);
            }
        }

        private void ProcessListEnumerableXmlElement(object obj, XmlMapping mapping, XElement elm)
        {
            var type = mapping.Property.PropertyType;
            var underlyingType = type.GetGenericArguments().First();

            var list = Activator.CreateInstance(type);

            var elms = mapping.AttributeValue.Select(a => elm.Elements(a)).First(x => x != null).ToList();

            Logger.Debug($"XElement contained {elms.Count} element(s)");

            foreach (var e in elms)
            {
                ((IList)list).Add(Deserialize(underlyingType, e));
            }

            mapping.Property.SetValue(obj, list);
        }

        private void ProcessNonListEnumerableXmlElement(object obj, XmlMapping mapping, XElement elm)
        {
            var attribute = mapping.Property.GetCustomAttribute(typeof(SplittableStringAttribute)) as SplittableStringAttribute;

            if (attribute != null)
            {
                var value = mapping.AttributeValue.Select(a => elm.Element(a)).FirstOrDefault(x => x != null);

                var str = value != null ? $"\"{value.Value}\"" : "null";
                Logger.Debug($"XElement contained {str}");

                value = NullifyMissingValue(value);

                object finalVal = value;

                if (value != null)
                {
                    finalVal = value.Value.Trim().Split(attribute.Character);
                }

                mapping.Property.SetValue(obj, finalVal);
            }
            else
                throw new NotSupportedException(); //todo: add an appropriate failure message
        }

        private void ProcessSingleXmlElement(object obj, XmlMapping mapping, XElement elm)
        {
            var type = mapping.Property.PropertyType;
            var value = mapping.GetSingleXElementAttributeValue(elm);

            //priority is priority_raw on the root group, priority everywhere else. we need an alt xmlelement
            //ok we have one, we now need to treat attributevalue as a list

            try
            {
                ProcessXmlText(obj, mapping, value);
            }
            catch (Exception ex) when (!(ex is XmlDeserializationException))
            {
                throw new XmlDeserializationException(mapping.Property.PropertyType, value?.ToString() ?? "null", ex);
            }
        }

        private XElement NullifyMissingValue(XElement value)
        {
            if (string.IsNullOrEmpty(value?.ToString()) || string.IsNullOrEmpty(value.Value))
            {
                return null;
            }

            return value;
        }

        private XAttribute NullifyMissingValue(XAttribute value)
        {
            if (string.IsNullOrEmpty(value?.ToString()) || string.IsNullOrEmpty(value.Value))
            {
                return null;
            }

            return value;
        }

        //todo: profile the performance of my method vs the .net method

        private void ProcessXmlAttribute(object obj, XmlMapping mapping, XElement elm)
        {
            var value = mapping.GetSingleXAttributeAttributeValue(elm);

            var str = value != null ? $"\"{value.Value}\"" : "null";
            Logger.Debug($"XAttribute contained {str}");

            value = NullifyMissingValue(value);
            var finalValue = value == null ? null : GetValue(mapping.Property.PropertyType, value.Value, elm);

            mapping.Property.SetValue(obj, finalValue);
        }

        private void ProcessXmlText(object obj, XmlMapping mapping, XElement elm)
        {
            var type = mapping.Property.PropertyType;

            var str = elm != null ? $"\"{elm.Value}\"" : "null";
            Logger.Debug($"XElement contained {str}");

            elm = NullifyMissingValue(elm);

            var finalValue = elm == null ? null : GetValue(type, elm.Value, elm);

            if (type.IsValueType && Nullable.GetUnderlyingType(type) == null && finalValue == null)
                throw new XmlDeserializationException($"An error occurred while attempting to deserialize XML element '{mapping.AttributeValue.First()}' to property '{mapping.Property.Name}': cannot assign 'null' to value type '{type.Name}'."); //value types cant be null

            mapping.Property.SetValue(obj, finalValue);
        }

        private object GetValue(Type type, object value, XElement elm)
        {
            if (type.IsPrimitive)
                return GetPrimitiveValue(type, value);
            else if (Nullable.GetUnderlyingType(type) != null) //if we're nullable, id say call the getvalue method again on the underlying type
            {
                var t = Nullable.GetUnderlyingType(type);
                return GetValue(t, value, elm);
            }
            else if (type == typeof (string))
            {
                return value.ToString();
            }
            else if (type.IsEnum)
            {
                var e = EnumHelpers.XmlToEnumAnyAttrib(value.ToString(), type, null, allowFlags: false, allowParse: false);

                if (e == null)
                {
                    var badXml = elm.ToString();

                    var name = elm.Name.ToString();

                    var index = name.LastIndexOf("_raw");

                    if (index != -1)
                    {
                        //We are the raw element, so get the normal element and add it
                        name = name.Substring(0, index);

                        var normalElm = elm.Parent?.Element(name);

                        if (normalElm != null)
                            badXml = normalElm + badXml;
                    }

                    var msg = elm.Parent?.Element("message");

                    if (msg != null)
                        badXml = badXml + msg.ToString().Replace("&lt;", "<").Replace("&gt;", ">");

                    throw new XmlDeserializationException($"Could not deserialize value '{value}' as it is not a valid member of type '{type}'. Could not process XML '{badXml}'");
                }

                return e;
            }
            else if (type == typeof (DateTime))
            {
                return DeserializationHelpers.ConvertPrtgDateTime(XmlConvert.ToDouble(value.ToString()));
            }
            else if (type == typeof (TimeSpan))
            {
                return DeserializationHelpers.ConvertPrtgTimeSpan(XmlConvert.ToDouble(value.ToString()));
            }
            return null;
        }

        private object GetPrimitiveValue(Type type, object value)
        {
            var str = value.ToString();

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32:
                    return XmlConvert.ToInt32(str);
                case TypeCode.Boolean:
                    return ToBoolean(str.ToLower());
                case TypeCode.Int16:
                    return XmlConvert.ToInt16(str);
                case TypeCode.Int64:
                    return XmlConvert.ToInt64(str);
                case TypeCode.Single:
                    return XmlConvert.ToSingle(str);
                case TypeCode.Double:
                    return ToDouble(str);
                case TypeCode.Decimal:
                    return XmlConvert.ToDecimal(str);
                case TypeCode.Char:
                    return XmlConvert.ToChar(str);
                case TypeCode.Byte:
                    return XmlConvert.ToByte(str);
                case TypeCode.SByte:
                    return XmlConvert.ToSByte(str);
                case TypeCode.UInt16:
                    return XmlConvert.ToUInt16(str);
                case TypeCode.UInt32:
                    return XmlConvert.ToUInt32(str);
                case TypeCode.UInt64:
                    return XmlConvert.ToUInt64(str);
            }

            throw new NotSupportedException(); //TODO - say the type is not deserializable
        }

        public static Boolean ToBoolean(string s)
        {
            s = TrimString(s);

            if (s == "-1")
                return true;

            return XmlConvert.ToBoolean(s);
        }

        internal static readonly char[] WhitespaceChars = { ' ', '\t', '\n', '\r' };

        // Trim a string using XML whitespace characters 
        internal static string TrimString(string value)
        {
            return value.Trim(WhitespaceChars);
        }

        //Custom ToDouble with culture specific formatting (for values retrieved from scraping HTML)
        public static double ToDouble(string s)
        {
            s = TrimString(s);
            if (s == "-INF")
                return double.NegativeInfinity;
            if (s == "INF")
                return double.PositiveInfinity;

            var numberStyle = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

            double dVal;

            //XML values should always be InvariantCulture. If value was scraped from HTML, value will be CurrentCulture PRTG Server.
            if (!double.TryParse(s, numberStyle, NumberFormatInfo.InvariantInfo, out dVal))
            {
                dVal = double.Parse(s, numberStyle, NumberFormatInfo.CurrentInfo);
            }

            if (dVal == 0 && s[0] == '-')
            {
                return -0d;
            }
            return dVal;
        }
    }
}
