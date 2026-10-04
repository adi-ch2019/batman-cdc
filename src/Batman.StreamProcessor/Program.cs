using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;

var handler = async (DynamoDBEvent dynamoEvent, Amazon.Lambda.Core.ILambdaContext context) =>
{
    return new Batman.StreamProcessor.Function().FunctionHandler(dynamoEvent, context);
};

await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
    .Build()
    .RunAsync();