using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.Azure.Functions.Worker;

namespace SecureIsolatedAzFunction
{

    public static class FunctionContextExtensions
    {
        public static void SetHttpResponseStatusCode(
            this FunctionContext context,
            HttpStatusCode statusCode)
        {
            var coreAssembly = Assembly.Load("Microsoft.Azure.Functions.Worker.Core");
            var featureInterfaceName = "Microsoft.Azure.Functions.Worker.Context.Features.IFunctionBindingsFeature";
            var featureInterfaceType = coreAssembly.GetType(featureInterfaceName);
            var bindingsFeature = context.Features.Single(
                f => f.Key.FullName == featureInterfaceType.FullName).Value;
            var invocationResultProp = featureInterfaceType.GetProperty("InvocationResult");

            var grpcAssembly = Assembly.Load("Microsoft.Azure.Functions.Worker.Grpc");
            var responseDataType = grpcAssembly.GetType("Microsoft.Azure.Functions.Worker.GrpcHttpResponseData");
            var responseData = Activator.CreateInstance(responseDataType, context, statusCode);

            invocationResultProp.SetMethod.Invoke(bindingsFeature, new object[] { responseData });
        }
    }
}