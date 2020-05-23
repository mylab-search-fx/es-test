using System;
using System.Net;
using System.Text;
using Nest;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace MyLab.Elastic.Test
{
    /// <summary>
    /// Logs ES events into test output
    /// </summary>
    public class TestEsLogger
    {
        public ITestOutputHelper Output { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="TestEsLogger"/>
        /// </summary>
        public TestEsLogger(ITestOutputHelper output)
        {
            Output = output;
        }

        public void Log(IResponse esResponse)
        {
            var call = esResponse.ApiCall;
            var sb = new StringBuilder();
            
            sb.AppendLine("===================================== REQUEST");
            sb.AppendLine();
            sb.AppendLine($"{call.HttpMethod} {call.Uri}");
            sb.AppendLine();

            if (call.RequestBodyInBytes != null)
            {
                var reqJson = Encoding.UTF8.GetString(call.RequestBodyInBytes);
                var formattedReqBody = FormatJson(reqJson);
                sb.AppendLine(formattedReqBody);
            }
            else
            {
                sb.AppendLine("[no request body]");
            }

            sb.AppendLine();
            sb.AppendLine("===================================== RESPONSE");
            sb.AppendLine();

            if (call.HttpStatusCode.HasValue)
            {
                HttpStatusCode statusCode = (HttpStatusCode) call.HttpStatusCode;
                sb.AppendLine($"{(int)statusCode} ({statusCode})");
            }
            else
            {
                sb.AppendLine($"[null]");
            }

            sb.AppendLine();

            if (call.ResponseBodyInBytes != null)
            {
                var respJson = Encoding.UTF8.GetString(call.ResponseBodyInBytes);
                var formattedRespBody = FormatJson(respJson);
                sb.AppendLine(formattedRespBody);
            }
            else
            {
                sb.AppendLine("[no response body]");
            }

            sb.AppendLine();
            sb.AppendLine("===================================== END");

            Output.WriteLine(sb.ToString());
        }

        private static string FormatJson(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
    }
}
