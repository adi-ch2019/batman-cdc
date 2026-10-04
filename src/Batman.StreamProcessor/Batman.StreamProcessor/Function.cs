using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Batman.StreamProcessor;

public class Function
{
    public StreamsEventResponse FunctionHandler(
        DynamoDBEvent dynamoEvent,
        ILambdaContext context)
    {
        var batchItemFailures = new List<StreamsEventResponse.BatchItemFailure>();
        
        context.Logger.LogInformation(
            $"Processing {dynamoEvent.Records.Count} stream records");

        foreach (var record in dynamoEvent.Records)
        {
            try
            {
                ProcessRecord(record, context);
            }
            catch (Exception ex)
            {
                context.Logger.LogError(
                    $"Failed record {record.Dynamodb.SequenceNumber}: {ex.Message}");
                
                // Report ONLY this record as failed
                batchItemFailures.Add(
                    new StreamsEventResponse.BatchItemFailure(
                        record.Dynamodb.SequenceNumber));
            }
        }

        return new StreamsEventResponse
        {
            BatchItemFailures = batchItemFailures
        };
    }

    private static void ProcessRecord(
        DynamoDBEvent.DynamodbStreamRecord record,
        ILambdaContext context)
    {
        var eventName = record.EventName; // INSERT, MODIFY, REMOVE
        var keys = record.Dynamodb.Keys;
        var newImage = record.Dynamodb.NewImage;
        var oldImage = record.Dynamodb.OldImage;

        var itemId = keys["Id"].S;
        
        context.Logger.LogInformation(
            $"CDC Event: {eventName} for item {itemId}");

        switch (eventName)
        {
            case "INSERT":
                context.Logger.LogInformation(
                    $"New item created: {itemId}");
                // Trigger downstream actions (e.g., send notification)
                break;

            case "MODIFY":
                context.Logger.LogInformation(
                    $"Item modified: {itemId}");
                // Detect specific field changes
                if (newImage != null && oldImage != null)
                {
                    var newName = newImage["Name"].S;
                    var oldName = oldImage["Name"].S;
                    
                    if (newName != oldName)
                    {
                        context.Logger.LogInformation(
                            $"Name changed: {oldName} -> {newName}");
                    }
                }
                break;

            case "REMOVE":
                context.Logger.LogInformation(
                    $"Item deleted: {itemId}");
                break;
        }
    }
}