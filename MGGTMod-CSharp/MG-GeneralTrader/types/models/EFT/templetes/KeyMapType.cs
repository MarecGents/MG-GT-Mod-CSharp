using SPTarkov.Server.Core.Models.Common;

namespace _MGMod.types.models.EFT.templetes;

public class MapKeyList
{
    public List<MongoId> MapBigmap { get; set; }
    public List<MongoId> MapShoreline { get; set; }
    public List<MongoId> MapRezervbase { get; set; }
    public List<MongoId> MapFactory4 { get; set; }
    public List<MongoId> MapWoods { get; set; }
    public List<MongoId> MapLighthouse { get; set; }
    public List<MongoId> MapInterchange { get; set; }
    public List<MongoId> MapLaboratory { get; set; }
    public List<MongoId> MapTarkovstreets { get; set; }
    public List<MongoId> MapSandbox { get; set; }
    public List<MongoId> MapLabyrinth { get; set; }
    public List<MongoId> MapUnknown { get; set; }
}