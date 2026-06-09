using System.Text.Json.Serialization;
using _MGGTmod.types.models.EFT.templetes;
using _MGGTmod.types.models.Paths;
using _MGGTmod.types.server;
using _MGGTmod.types.utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;

namespace _MGGTmod.types.services;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class TestServices
{
    private readonly ISptLogger<TestServices> logger;
    private readonly MGUtils mGUtils;
    private TemplatesServer templatesServer;

    public TestServices(
        ISptLogger<TestServices> _logger,
        MGUtils _mGUtils,
        TemplatesServer _templatesServer
    )
    {
        logger = _logger;
        mGUtils = _mGUtils;
        templatesServer = _templatesServer;
    }

    public void Initialize()
    {
        Dictionary<MongoId, TemplateItem> Items = templatesServer.GetItems();
        TemplateItem item = new TemplateItem();
        Items.TryGetValue("67069c8cee8138ed2f05ad34", out item);
        mGUtils.WriteFile("item.json", mGUtils.Serialize(item));
    }
    
}

public class TestClassJson1
{
    public string _id { get; set; }
}

public class TestClassJson2
{
    public MongoId _id { get; set; }
}

public record TestRecordJson1
{
    private string _id;

    [JsonPropertyName("_id")]
    public virtual required string Id
    {
        get => _id;
        set => _id = value == null ? null : new string(value);
    }
}

public record TestRecordJson2
{
    private MongoId _id;

    [JsonPropertyName("_id")]
    public virtual required MongoId Id
    {
        get => _id;
        set => _id = value == null ? null : new MongoId(value);
    }
}
