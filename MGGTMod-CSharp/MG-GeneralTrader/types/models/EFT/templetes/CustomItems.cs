using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;

namespace _MGGTmod.types.models.EFT.templetes;

public class BrothersItem : ICustomItemBuffs
{
    public MongoId newId { get; set; }
    public MongoId itemTplToClone { get; set; }
    public TemplateItemProperties overrideProperties { get; set; }
    public double? fleaPriceRoubles { get; set; }
    public BroLocal locales { get; set; }
    public Dictionary<string, List<Buff>>? Buffs { get; set; }
}
public class BroLocal
{
    public LocaleDetails ch {  get; set; }
}
public class SuperItem : ICustomItemBuffs
{
    public MongoId tpl {  get; set; }
    public SuperItems items { get; set; }
    public HandbookItem handbook { get; set; }
    public List<Item>? assort {  get; set; }
    public Dictionary<string, List<Buff>>? Buffs { get; set ; }
}
public class SuperItems
{
    public MongoId _id { get; set; }
    public string _name { get; set; }
    public TemplateItemProperties _props { get; set; }
    public string? _parent { get; set; }
    public string? _type { get; set; }
    public string? _proto { get; set; }
}
public class MGItem : ICustomItemBuffs
{
    public MGItems items { get; set; }
    public double price { get; set; }
    public LocaleDetails description { get; set; }
    public MongoId? toTraderId { get; set; }
    public bool? isSold { get; set; }
    public int? loyal_level { get; set; }
    public List<Item>? assort { get; set; }
    public MongoId? currency { get; set; }
    public Dictionary<string, List<Buff>>? Buffs { get; set; }
    public MongoId? HandbookId { get; set; }
}
public class MGItems
{
    public MongoId newId { get; set; }
    public MongoId cloneId { get; set; }
    public TemplateItemProperties _props { get; set; }
}
public class CustomTraderItems
{
    public TemplateItem item { get; set; }
    public string origin;
    public List<string>? Type { get; set; }
}