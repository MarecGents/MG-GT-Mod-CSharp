using System.Text.Json.Serialization;
using _MGMod.types.models.EFT.templetes;
using _MGMod.types.models.Paths;
using _MGMod.types.utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Utils;

namespace _MGMod.types.services;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class TestServices
{
    private ISptLogger<TestServices> logger;
    private MGUtils mGUtils;
    
    public TestServices(
        ISptLogger<TestServices> _logger,
        MGUtils _mGUtils
    )
    {
        logger = _logger;
        mGUtils = _mGUtils;
    }

    public void Initialize()
    {
        logger.Error(MongoId.IsValidMongoId("8ef5f317245977243854e041").ToString());
        var test1json = mGUtils.GetJsonDataFromFile<SuperItem>(
            new PathType
            {
                Path = Paths.SuperItemPath,
                FileName = "超级狗牌包.json"
            });
        logger.Error(mGUtils.Serialize(test1json));
        
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
        get {return _id;}
        set {_id = value== null ? null : new string(value);}
    }
}

public record TestRecordJson2
{
    private MongoId _id;

    [JsonPropertyName("_id")]
    public virtual required MongoId Id
    {
        get {return _id;}
        set {_id = value== null ? null : new MongoId(value);}
    }
}
