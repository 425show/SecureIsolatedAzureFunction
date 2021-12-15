using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace SecureIsolatedAzFunction
{
    public class GetGraphData
    {
        private readonly AuthenticationProvider _authentication;
        public GetGraphData(AuthenticationProvider authentication)
        {
            _authentication = authentication;
        }

        [Function("GetData")]
        public static HttpResponseData Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req,
            FunctionContext context)
        {
            var logger = context.GetLogger<GetGraphData>();
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteString("Success!");
            return response;
        }
    }
}
