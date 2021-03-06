﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrtgAPI.Parameters;
using PrtgAPI.Request;

namespace PrtgAPI.Tests.UnitTests.InfrastructureTests
{
    [TestClass]
    public class PrtgUrlTests
    {
        [TestMethod]
        public void PrtgUrl_Server_Prefix_Http()
        {
            Server_Prefix("http://prtg.example.com", "http://prtg.example.com");
        }

        [TestMethod]
        public void PrtgUrl_Server_Prefix_Https()
        {
            Server_Prefix("https://prtg.example.com", "https://prtg.example.com");
        }

        [TestMethod]
        public void PrtgUrl_Server_Prefix_None()
        {
            Server_Prefix("prtg.example.com", "https://prtg.example.com");
        }

        [TestMethod]
        public void PrtgUrl_SpecifiedNoPass_Yields_PassHash()
        {
            var url = CreateUrl(new Parameters.Parameters());

            Assert.IsTrue(url.Contains("passhash=password"));
            Assert.IsFalse(url.Contains("password=password"));
        }

        [TestMethod]
        public void PrtgUrl_SpecifiedPassHash_Yields_PassHash()
        {
            var url = CreateUrl(new Parameters.Parameters
            {
                [Parameter.PassHash] = "password"
            });

            Assert.IsTrue(url.Contains("passhash=password"));
            Assert.IsFalse(url.Contains("password=password"));
        }

        [TestMethod]
        public void PrtgUrl_SpecifiedPassword_Yields_Password()
        {
            var url = CreateUrl(new Parameters.Parameters
            {
                [Parameter.Password] = "password"
            });

            Assert.IsFalse(url.Contains("passhash=password"));
            Assert.IsTrue(url.Contains("password=password"));
        }

        [TestMethod]
        public void PrtgUrl_MultiParameter_WithoutIEnumerable_Equals_WithIEnumerable()
        {
            var urlWithoutArray = CreateUrl(new Parameters.Parameters
            {
                [Parameter.FilterXyz] = new SearchFilter(Property.Name, "dc1")
            });

            var urlWithArray = CreateUrl(new Parameters.Parameters
            {
                [Parameter.FilterXyz] = new[] {new SearchFilter(Property.Name, "dc1")}
            });

            Assert.IsTrue(urlWithArray == urlWithoutArray);
        }

        [TestMethod]
        public void PrtgUrl_MultiValue_WithoutIEnumerable_Equals_WithIEnumerable()
        {
            var urlWithoutArray = CreateUrl(new Parameters.Parameters
            {
                [Parameter.Columns] = Property.Id
            });

            var urlWithArray = CreateUrl(new Parameters.Parameters
            {
                [Parameter.Columns] = new[] {Property.Id }
            });

            Assert.IsTrue(urlWithArray == urlWithoutArray);
        }

        [TestMethod]
        public void PrtgUrl_CustomParameter_WithoutIEnumerable_Equals_WithIEnumerable()
        {
            var urlWithoutArray = CreateUrl(new Parameters.Parameters
            {
                [Parameter.Custom] = new CustomParameter("foo", "bar")
            });

            var urlWithArray = CreateUrl(new Parameters.Parameters
            {
                [Parameter.Custom] = new[] { new CustomParameter("foo", "bar") }
            });

            Assert.IsTrue(urlWithArray == urlWithoutArray);
        }

        [TestMethod]
        public void PrtgUrl_ParameterValue_With_EnumFlags()
        {
            //Specifying FilterXyz doesn't actually make sense here (we should be using a SearchFilter) however
            //the point of the test is to validate flag parsing

            var flagsUrl = CreateUrl(new Parameters.Parameters
            {
                [Parameter.FilterXyz] = new SearchFilter(Property.Status, Status.Paused)
            });

            var manualUrl = CreateUrl(new Parameters.Parameters
            {
                [Parameter.FilterXyz] =
                    new[]
                    {
                        Status.PausedByUser, Status.PausedByDependency, Status.PausedBySchedule, Status.PausedByLicense, 
                        Status.PausedUntil
                    }.ToList().Select(s => new SearchFilter(Property.Status, s)).ToArray()
            });

            Assert.IsTrue(flagsUrl == manualUrl);
        }

        [TestMethod]
        public void PrtgUrl_SearchFilter_With_EnumFlags()
        {
            var flagsUrl = CreateUrl(new Parameters.Parameters
            {
                [Parameter.FilterXyz] = new SearchFilter(Property.Status, new[] { Status.Paused })
            });

            var manualUrl = CreateUrl(new Parameters.Parameters
            {
                [Parameter.FilterXyz] = new SearchFilter(Property.Status,
                    new[]
                    {
                        Status.PausedByUser, Status.PausedByDependency, Status.PausedBySchedule,
                        Status.PausedByLicense, Status.PausedUntil
                    })

            });

            Assert.IsTrue(flagsUrl == manualUrl);
        }

        [TestMethod]
        public void PrtgUrl_Throws_WhenCustomParameterValueIsWrongType()
        {
            try
            {
                var url = CreateUrl(new Parameters.Parameters
                {
                    [Parameter.Custom] = 3
                });
            }
            catch (ArgumentException ex)
            {
                if (!ex.Message.Contains("Expected one or more objects of type CustomParameter, however argument was of type System.Int32"))
                    throw;
            }
            
        }

        private string CreateUrl(Parameters.Parameters parameters)
        {
            var url = new PrtgUrl(new ConnectionDetails("prtg.example.com", "username", "password"), XmlFunction.TableData, parameters);

            return url.Url;
        }

        private void Server_Prefix(string server, string prefixedServer)
        {
            var url = new PrtgUrl(new ConnectionDetails(server, "username", "password"), XmlFunction.TableData, new Parameters.Parameters());

            Assert.IsTrue(url.Url.StartsWith(prefixedServer));
        }
    }
}
