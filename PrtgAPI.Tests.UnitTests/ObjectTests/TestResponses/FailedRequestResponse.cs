﻿using System;
using System.Net;
using System.Threading.Tasks;
using PrtgAPI.Tests.UnitTests.InfrastructureTests.Support;

namespace PrtgAPI.Tests.UnitTests.ObjectTests.TestResponses
{
    class FailedRequestResponse : IWebResponse
    {
        private string responseText;
        private string addr;

        public FailedRequestResponse(HttpStatusCode statusCode, string responseText, string address)
        {
            StatusCode = statusCode; this.responseText = responseText;
            addr = address;
        }

        public string GetResponseText(ref string address)
        {
            if (addr != null)
                address = addr;

            return responseText;
        }

        public Task<string> GetResponseTextStream(string address)
        {
            throw new NotImplementedException();
        }

        public HttpStatusCode StatusCode { get; set; }
    }
}
