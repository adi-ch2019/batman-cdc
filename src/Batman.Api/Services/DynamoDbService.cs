using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Batman.Api.Models;

namespace Batman.Api.Services;

public class DynamoDbService
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName;

    public DynamoDbService(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
        _tableName = Environment.GetEnvironmentVariable("TABLE_NAME") 
                     ?? "batman-cdc-table";
    }

    public async Task<BatmanItem> SaveItemAsync(string name)
    {
        var table = Table.LoadTable(_dynamoDb, _tableName);
        var id = Guid.NewGuid().ToString();
        
        var doc = new Document
        {
            ["Id"] = id,
            ["Name"] = name,
            ["LastModified"] = DateTime.UtcNow.ToString("o")
        };

        await table.PutItemAsync(doc);
        
        return new BatmanItem(id, name, doc["LastModified"]);
    }

    public async Task<List<BatmanItem>> GetAllItemsAsync()
    {
        var table = Table.LoadTable(_dynamoDb, _tableName);
        var search = table.Scan(new ScanFilter());
        var documents = await search.GetNextSetAsync();

        return documents.Select(doc => new BatmanItem(
            doc["Id"].AsString(),
            doc["Name"].AsString(),
            doc["LastModified"].AsString()
        )).ToList();
    }
}