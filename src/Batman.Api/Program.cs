using Amazon.DynamoDBv2;
using Amazon.Lambda.AspNetCoreServer.Hosting;
using Batman.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Tells Lambda to host this ASP.NET Core app for an HTTP API (API Gateway v2)
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
builder.Services.AddSingleton<DynamoDbService>();

var app = builder.Build();

app.MapGet("/items", async (DynamoDbService service) =>
{
    var items = await service.GetAllItemsAsync();
    return Results.Ok(items);
});

app.MapPost("/items", async (CreateItemRequest request, DynamoDbService service) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Name is required" });
    var item = await service.SaveItemAsync(request.Name);
    return Results.Ok(item);
});

app.Run();

public record CreateItemRequest(string Name);