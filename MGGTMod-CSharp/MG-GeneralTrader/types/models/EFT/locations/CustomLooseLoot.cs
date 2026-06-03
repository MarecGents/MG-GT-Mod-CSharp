using _MGMod.types.models.EFT.traders;
using SPTarkov.Server.Core.Models.Eft.Common;

namespace _MGMod.types.models.EFT.locations;

public class CustomLooseLoot
{
    public SpawnpointCount? spawnpointCount { get; set; }
    public List<Spawnpoint>? spawnpointsForced { get; set; }
    public List<Spawnpoint>? spawnpoints { get; set; }
}

public class CustomTraderLooseLoot
{
    public SpawnpointCount? spawnpointCount { get; set; }
    public List<CustomSpawnpoint>? spawnpointsForced { get; set; }
    public List<CustomSpawnpoint>? spawnpoints { get; set; }
}

public class CustomSpawnpoint
{
    public string? locationId { get; set; }

    public double? probability { get; set; }

    public CustomSpawnpointTemplate? template { get; set; }

    public IEnumerable<LooseLootItemDistribution>? itemDistribution { get; set; }
}

public class CustomSpawnpointTemplate
{
    public string? Id { get; set; }
    public bool? IsContainer { get; set; }
    public bool? useGravity { get; set; }
    public bool? randomRotation { get; set; }
    public XYZ? Position { get; set; }
    public XYZ? Rotation { get; set; }
    public bool? IsGroupPosition { get; set; }
    public IEnumerable<GroupPosition>? GroupPositions { get; set; }
    public bool? IsAlwaysSpawn { get; set; }
    public string? Root {get; set;}
    public List<CustomSptLootItem>? Items { get; set; }
}

public class CustomSptLootItem : ItemString
{
    public string? composedKey { get; set; }
}