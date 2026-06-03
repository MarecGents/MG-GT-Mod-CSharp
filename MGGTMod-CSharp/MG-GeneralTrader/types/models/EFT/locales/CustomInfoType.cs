using SPTarkov.Server.Core.Models.Common;

namespace _MGMod.types.models.EFT.locales;

public class GeneralInfo
{
    public required string Id { get; set; }
    public required string Desc { get; set; }
}
public class ItemsInfo
{
    public required string Id { get; set; }
    public required ItemsDesc Desc { get; set; }
}
public class TraderInfo
{
    public required string Id { get; set; }
    public required TraderDesc Desc { get; set; }
}
public class QuestInfo
{
    public required string Id { get; set; }
    public required QuestDesc Desc { get; set; }
}
public class ItemsDesc
{
    public required string Name { get; set; }
    public required string ShortName { get; set; }
    public required string Description { get; set; }
}
public class TraderDesc
{
    public required string FullName { get; set; }
    public required string FirstName { get; set; }
    public required string Nickname { get; set; }
    public required string Location { get; set; }
    public required string Description { get; set; }
}
public class QuestDesc
{
    public required string name { get; set; }
    public required string description { get; set; }
    public string? startedMessageText { get; set; }
    public string? successMessageText { get; set; }
    public string? failMessageText { get; set; }
    public string? changeQuestMessageText { get; set; }
    public string? acceptPlayerMessage { get; set; }
    public string? completePlayerMessage { get; set; }
    public string? declinePlayerMessage { get; set; }
    public Dictionary<MongoId, string>? other { get; set; }
}

public class AnyInfo : Dictionary<string, string>
{
}