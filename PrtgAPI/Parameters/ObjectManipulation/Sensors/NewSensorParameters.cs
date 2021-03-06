﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PrtgAPI.Attributes;
using PrtgAPI.Helpers;
using PrtgAPI.Request;

namespace PrtgAPI.Parameters
{
    /// <summary>
    /// <para type="description">Represents parameters used to construct a <see cref="PrtgUrl"/> for adding new <see cref="Sensor"/> objects.</para>
    /// </summary>
    public abstract class NewSensorParameters : Parameters
    {
        /// <summary>
        /// The name to use for this sensor.
        /// </summary>
        [RequireValue(true)]
        public string Name
        {
            get { return (string)GetCustomParameter(ObjectProperty.Name); }
            set { SetCustomParameter(ObjectProperty.Name, value); } 
        }

        /// <summary>
        /// Whether to inherit notification triggers from the parent object.
        /// </summary>
        public bool? InheritTriggers
        {
            get { return GetCustomParameterBool(ObjectProperty.InheritTriggers); }
            set { SetCustomParameterBool(ObjectProperty.InheritTriggers, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewSensorParameters"/> class.
        /// </summary>
        /// <param name="sensorName">The name to use for this sensor.</param>
        /// <param name="inheritTriggers">Whether to inherit notification triggers from the parent object.</param>
        /// <param name="sensorType">The type of sensor these parameters will create.</param>
        public NewSensorParameters(string sensorName, bool inheritTriggers, object sensorType)
        {
            if (string.IsNullOrEmpty(sensorName))
                throw new ArgumentException($"{nameof(sensorName)} cannot be null or empty", nameof(sensorName));

            if (string.IsNullOrEmpty(sensorType?.ToString()))
                throw new ArgumentException($"{nameof(sensorType)} cannot be null or empty", nameof(sensorType));

            Name = sensorName;
            InheritTriggers = inheritTriggers;
            this[Parameter.SensorType] = sensorType;
        }

        #region GetCustomParameter

        /// <summary>
        /// Retrieve the value of a property from the underlying parameter set.
        /// </summary>
        /// <param name="property">The property whose value should be retrieved</param>
        /// <returns>The original unserialized value.</returns>
        protected object GetCustomParameter(ObjectProperty property) =>
            GetCustomParameterInternal(GetObjectPropertyName(property));

        [ExcludeFromCodeCoverage]
        internal object GetCustomParameter(ObjectPropertyInternal property) =>
            GetCustomParameterInternal(GetObjectPropertyInternalName(property));

        #endregion
        #region GetCustomParameterEnumXml

        /// <summary>
        /// Retrieve the original value of a serialized enum property from the underlying parameter set.
        /// </summary>
        /// <typeparam name="T">The type of enum to retrieve.</typeparam>
        /// <param name="property">The property whose value should be retrieved.</param>
        /// <returns>The original unserialized value.</returns>
        protected object GetCustomParameterEnumXml<T>(ObjectProperty property) =>
            GetCustomParameterEnumXml<T>(GetObjectPropertyName(property));

        [ExcludeFromCodeCoverage]
        internal object GetCustomParameterEnumXml<T>(ObjectPropertyInternal property) =>
            GetCustomParameterEnumXml<T>(GetObjectPropertyInternalName(property));

        /// <summary>
        /// Retrieve the original value of a serialized enum property from the underlying parameter set, using its raw serialized name.
        /// </summary>
        /// <typeparam name="T">The type of enum to retrieve.</typeparam>
        /// <param name="name">The raw name of the parameter</param>
        /// <returns>The original unserialized value.</returns>
        protected object GetCustomParameterEnumXml<T>(string name)
        {
            var value = GetCustomParameterInternal(name);

            if (value == null)
                return null;

            return value.ToString().XmlToEnum<T>();
        }

        #endregion
        #region GetCustomParameterBool

        /// <summary>
        /// Retrieve the original value of a serialized boolean property from the underlying parameter set.
        /// </summary>
        /// <param name="property">The property whose value should be retrieved.</param>
        /// <returns>The original unserialized value.</returns>
        protected bool? GetCustomParameterBool(ObjectProperty property) =>
            GetCustomParameterBool(GetObjectPropertyName(property));

        [ExcludeFromCodeCoverage]
        internal bool? GetCustomParameterBool(ObjectPropertyInternal property) =>
            GetCustomParameterBool(GetObjectPropertyInternalName(property));

        /// <summary>
        /// Retrieve the original value of a serialized boolean property from the underlying parameter set, using its raw serialized name.
        /// </summary>
        /// <param name="name">The raw name of the parameter</param>
        /// <returns>The original unserialized value.</returns>
        protected bool? GetCustomParameterBool(string name)
        {
            var value = GetCustomParameterInternal(name);

            if (value == null)
                return null;

            return Convert.ToBoolean(value);
        }

        #endregion
        #region GetCustomParameterArray

        /// <summary>
        /// Retrieve the original value of a serialized array property from the underlying parameter set.
        /// </summary>
        /// <param name="property">The parameter to retrieve</param>
        /// <param name="delim">The value that was used to combine the array when originally serialized via <see cref="SetCustomParameterArray(ObjectProperty, string[], char)"/> </param>
        /// <returns>The original unserialized value.</returns>
        protected string[] GetCustomParameterArray(ObjectProperty property, char delim) =>
            GetCustomParameterArray(GetObjectPropertyName(property), delim);

        [ExcludeFromCodeCoverage]
        internal string[] GetCustomParameterArray(ObjectPropertyInternal property, char delim) =>
            GetCustomParameterArray(GetObjectPropertyInternalName(property), delim);

        /// <summary>
        /// Retrieve the original value of a serialized array property from the underlying parameter set, using its raw serialized name.
        /// </summary>
        /// <param name="name">The raw name of the parameter</param>
        /// <param name="delim">The value that was used to combine the array when originally serialized via <see cref="SetCustomParameterArray(string, string[], char)"/> </param>
        /// <returns>The original unserialized value.</returns>
        protected string[] GetCustomParameterArray(string name, char delim)
        {
            var value = GetCustomParameterInternal(name);

            var array = value?.ToString().Split(delim);

            return array;
        }

        #endregion

        /// <summary>
        /// Retrieve a parameter from the underlying parameter set using its raw, serialized name. If set does not contain a value for the specified parameter, null will be returned.
        /// </summary>
        /// <param name="name">The raw name of the parameter</param>
        /// <returns>The parameter's corresponding value. If the parameter was not found in the set, this value is null</returns>
        protected object GetCustomParameterInternal(string name)
        {
            var index = GetCustomParameterIndex(name);

            if (index == -1)
                return null;
            else
                return Parameters[index].Value;
        }

        #region SetCustomParameter

        /// <summary>
        /// Store the value of a property in the underlying parameter set.
        /// </summary>
        /// <param name="property">The property whose value should be stored.</param>
        /// <param name="value">The value to serialize.</param>
        protected void SetCustomParameter(ObjectProperty property, object value) =>
            SetCustomParameterInternal(GetObjectPropertyName(property), value);

        [ExcludeFromCodeCoverage]
        internal void SetCustomParameter(ObjectPropertyInternal property, object value) =>
            SetCustomParameterInternal(GetObjectPropertyInternalName(property), value);

        #endregion
        #region SetCustomParameterEnumXml

        /// <summary>
        /// Store the serialized value of an enum property in the underlying parameter set.
        /// </summary>
        /// <typeparam name="T">The type of enum to serialize.</typeparam>
        /// <param name="property">The property whose value should be stored.</param>
        /// <param name="enum">The enum to serialize.</param>
        protected void SetCustomParameterEnumXml<T>(ObjectProperty property, T @enum) where T : struct =>
            SetCustomParameterEnumXml(GetObjectPropertyName(property), @enum);

        [ExcludeFromCodeCoverage]
        internal void SetCustomParameterEnumXml<T>(ObjectPropertyInternal property, T @enum) where T : struct =>
            SetCustomParameterEnumXml(GetObjectPropertyInternalName(property), @enum);

        /// <summary>
        /// Store the serialized value of an enum property in the underlying parameter set, using its raw serialized name.
        /// </summary>
        /// <typeparam name="T">The type of enum to serialize.</typeparam>
        /// <param name="name">The raw name of the parameter.</param>
        /// <param name="enum">The enum to serialize.</param>
        protected void SetCustomParameterEnumXml<T>(string name, T @enum) where T : struct =>
            SetCustomParameterInternal(name, ((Enum)(object)@enum).EnumToXml());

        #endregion
        #region SetCustomParameterBool

        /// <summary>
        /// Store the serialized value of a boolean property in the underlying parameter set.
        /// </summary>
        /// <param name="property">The property whose value should be stored.</param>
        /// <param name="value">The bool to serialize.</param>
        protected void SetCustomParameterBool(ObjectProperty property, bool? value) =>
            SetCustomParameterBool(GetObjectPropertyName(property), value);

        [ExcludeFromCodeCoverage]
        internal void SetCustomParameterBool(ObjectPropertyInternal property, bool? value) =>
            SetCustomParameterBool(GetObjectPropertyInternalName(property), value);

        /// <summary>
        /// Store the serialized value of a boolean property in the underlying parameter set, using its raw serialized name.
        /// </summary>
        /// <param name="name">The raw name of the parameter.</param>
        /// <param name="value">The bool to serialize.</param>
        protected void SetCustomParameterBool(string name, bool? value) =>
            SetCustomParameterInternal(name, Convert.ToInt32(value));

        #endregion
        #region SetCustomParameterArray

        /// <summary>
        /// Store the serialized value of an array property in the underlying parameter set.
        /// </summary>
        /// <param name="property">The property whose value should be stored.</param>
        /// <param name="value">The array to serialize.</param>
        /// <param name="delim">The character that should delimit each array entry.</param>
        protected void SetCustomParameterArray(ObjectProperty property, string[] value, char delim) =>
            SetCustomParameterArray(GetObjectPropertyName(property), value, delim);

        [ExcludeFromCodeCoverage]
        internal void SetCustomParameterArray(ObjectPropertyInternal property, string[] value, char delim) =>
            SetCustomParameterArray(GetObjectPropertyInternalName(property), value, delim);

        /// <summary>
        /// Store the serialized value of an array property in the underlying parameter set, using its raw serialized name.
        /// </summary>
        /// <param name="name">The raw name of the parameter.</param>
        /// <param name="value">The array to serialize.</param>
        /// <param name="delim">The character that should delimit each array entry.</param>
        protected void SetCustomParameterArray(string name, string[] value, char delim)
        {
            var str = string.Join(delim.ToString(), value);

            SetCustomParameterInternal(name, str);
        }

        #endregion

        /// <summary>
        /// Store the value of a property in the underlying set, using its raw serialized name.
        /// </summary>
        /// <param name="name">The raw name of the parameter.</param>
        /// <param name="value">The value to store.</param>
        protected void SetCustomParameterInternal(string name, object value)
        {
            var parameter = new CustomParameter(name, value);

            var index = GetCustomParameterIndex(name);

            if (index == -1)
                Parameters.Add(parameter);
            else
                Parameters[index] = parameter;
        }

        #region Helpers

        private string GetObjectPropertyName(ObjectProperty property)
        {
            var info = BaseSetObjectPropertyParameters<ObjectProperty>.GetPropertyInfoViaTypeLookup(property);
            var name = BaseSetObjectPropertyParameters<ObjectProperty>.GetParameterNameStatic(property, info);

            return name;
        }

        [ExcludeFromCodeCoverage]
        private string GetObjectPropertyInternalName(ObjectPropertyInternal property)
        {
            return $"{property.GetDescription()}_";
        }

        private int GetCustomParameterIndex(string name)
        {
            var index = Parameters.FindIndex(a => a.Name == name);

            return index;
        }

        #endregion

        private List<CustomParameter> Parameters
        {
            get
            {
                if (this[Parameter.Custom] == null)
                    this[Parameter.Custom] = new List<CustomParameter>();

                return (List<CustomParameter>)this[Parameter.Custom];
            }
        }
    }

}
