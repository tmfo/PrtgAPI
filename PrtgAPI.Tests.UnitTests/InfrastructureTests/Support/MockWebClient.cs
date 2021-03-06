﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PrtgAPI.Request;

namespace PrtgAPI.Tests.UnitTests.InfrastructureTests.Support
{
    class MockWebClient : IWebClient
    {
        IWebResponse response;

        public MockWebClient(IWebResponse response)
        {
            this.response = response;
        }

        public Task<HttpResponseMessage> GetSync(string address)
        {
            if (response.StatusCode == 0)
                response.StatusCode = HttpStatusCode.OK;

            var message = new HttpResponseMessage(response.StatusCode)
            {
                Content = new StringContent(response.GetResponseText(ref address)),
                RequestMessage = new HttpRequestMessage()
                {
                    RequestUri = new Uri(address)
                }
            };

            return Task.FromResult(message);
        }

        public async Task<HttpResponseMessage> GetAsync(string address)
        {
            if (response.StatusCode == 0)
                response.StatusCode = HttpStatusCode.OK;

            var stack = new System.Diagnostics.StackTrace();

            var method = stack.GetFrames().Last(f => f.GetMethod().Module.Name == "PrtgAPI.dll").GetMethod();

            var responseStr = string.Empty;

            if (method.Name.StartsWith("Stream"))
            {
                responseStr = await response.GetResponseTextStream(address).ConfigureAwait(false);
            }
            else
            {
                //If the method is in fact async, or is called as part of a streaming method, we execute the request as async
                //This implies we do not consider nested streaming methods to be an implemented scenario
                responseStr = await Task.FromResult(response.GetResponseText(ref address)).ConfigureAwait(false);
            }
            //we should check whether the method is a streamer or an async, and if its async we should to task.fromresult

            return new HttpResponseMessage(response.StatusCode)
            {
                Content = new StringContent(responseStr),
                RequestMessage = new HttpRequestMessage()
                {
                    RequestUri = new Uri(address)
                }
            };
        }
    }
}
