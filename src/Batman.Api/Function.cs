using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Batman.Api.Services;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Batman.Api;

public class Function
{
    private readonly DynamoDbService _service;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public Function()
    {
        _service = new DynamoDbService(new Amazon.DynamoDBv2.AmazonDynamoDBClient());
    }

    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(
        APIGatewayHttpApiV2ProxyRequest request,
        ILambdaContext context)
    {
        return request.RouteKey switch
        {
            "GET /items" => await GetAllItems(),
            "POST /items" => await CreateItem(request),
            _ => NotFound()
        };
    }

    private async Task<APIGatewayHttpApiV2ProxyResponse> GetAllItems()
    {
        var items = await _service.GetAllItemsAsync();
        return Ok(JsonSerializer.Serialize(items, JsonOptions));
    }

    private async Task<APIGatewayHttpApiV2ProxyResponse> CreateItem(
        APIGatewayHttpApiV2ProxyRequest request)
    {
        var body = JsonSerializer.Deserialize<CreateItemRequest>(
            request.Body ?? "{}", JsonOptions);
        
        if (string.IsNullOrWhiteSpace(body?.Name))
            return BadRequest("Name is required");

        var item = await _service.SaveItemAsync(body.Name);
        return Ok(JsonSerializer.Serialize(item, JsonOptions));
    }

    private static APIGatewayHttpApiV2ProxyResponse Ok(string body) => new()
    {
        StatusCode = 200,
        Body = body,
        Headers = new Dictionary<string, string> 
        { 
            { "Content-Type", "application/json" } 
        }
    };

    private static APIGatewayHttpApiV2ProxyResponse NotFound() => new()
    {
        StatusCode = 404,
        Body = "{\"error\":\"Not found\"}",
        Headers = new Dictionary<string, string> 
        { 
            { "Content-Type", "application/json" } 
        }
    };

    private static APIGatewayHttpApiV2ProxyResponse BadRequest(string message) => new()
    {
        StatusCode = 400,
        Body = JsonSerializer.Serialize(new { error = message }),
        Headers = new Dictionary<string, string> 
        { 
            { "Content-Type", "application/json" } 
        }
    };
}

public record CreateItemRequest(string Name);