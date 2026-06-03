using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace _MGMod.types.models.EFT.traders;

public class CustomItemAssorts
{
    public List<Item> assort { get; set; }
    public double price { get; set; }
    public int loyal_level_items { get; set; }
    public MongoId traderId { get; set; }
    public MongoId currency { get; set; }
}

public class TraderAssortStringId
{
    public double? NextResupply { get; set; }
    public List<ItemString> items { get; set; }
    public Dictionary<string, List<List<BarterScheme>>> barter_scheme { get; set; }
    public Dictionary<string, int> loyal_level_items { get; set; }
    
}

public class ItemString
{
    public string _id { get; set; }
    public string _tpl { get; set; }
    public string parentId { get; set; }
    public string slotId { get; set; }
    public object? location  { get; set; }
    public string? desc { get; set; }
    public Upd? upd { get; set; }
}

