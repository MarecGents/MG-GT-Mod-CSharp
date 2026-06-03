using _MGGTmod.types.models.EFT.locales;
using _MGGTmod.types.models.EFT.locations;
using _MGGTmod.types.models.EFT.templetes;
using SPTarkov.Server.Core.Loaders;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace _MGGTmod.types.models.EFT.traders;

public class CustomTraders
{
    public Dictionary<string, CustomTraderItems>? items {  get; set; }
    public CustomTraderLocales? locales { get; set; }
    public CustomTraderLocation? location { get; set; }
    public CustomTraderTemplates? templates { get; set; }
    public Trader? traderData { get; set; }
    public BundleManifest? bundles { get; set; }
    public CustomGlobals? globals { get; set; }
    public CustomTraderInfo traderInfo { get; set; }
}

public class CustomTraderLocales
{
    public Dictionary<string, ItemsDesc>? itemsdescription { get; set; }
    public Dictionary<string, QuestDesc>? mail { get; set; }
}
public class CustomTraderLocation
{
    public Dictionary<string, CustomLooseLoot>? looseLoot { get; set; }
}

public class CustomTraderTemplates
{
    public List<HandbookItem>? handbook { get; set; }
    public Dictionary<string, Quest>? quests { get; set; }
}

public class CustomTraderInfo
{
    public bool enable {  get; set; }
    public string _id { get; set; }
    public string name { get; set; }
    public TraderDesc locales {  get; set; }
    public CustomTraderInsurance? insurance { get; set; }
    public CustomTraderRepair? repair { get; set; }
    public CustomTraderLoyaltyLevels? loyaltyLevels { get; set; }
    public int? discount { get; set; }
    public bool? medic { get; set; }
    public MinMax<int>? updateTime { get; set; }
    public bool? unlockedDefault { get; set; }
    
}

public class CustomTraderInsurance
{
    public bool enable { get; set; }
    public int minreturntime { get; set; }
    public int maxreturntime { get; set; }
    public int pay { get; set; }
    public int chance { get; set; }
    public int storageTime { get; set; }
    public Dictionary<string, List<string>?>? Message { get; set; }
}

public class CustomTraderRepair
{
    public bool enable { get; set; }
    public int coefficient { get; set; }
    public int quality { get; set; }
}

public class CustomTraderLoyaltyLevels
{
    public CustomTraderLoyaltyLevelsDesc? description { get; set; }
    public List<TraderLoyaltyLevel>? range { get; set; }
}

public class CustomTraderLoyaltyLevelsDesc
{
    public string main { get; set; }
    public string minLevel {get;set;}
    public string minSalesSum {get;set;}
    public string minStanding {get;set;}
    public string buy_price_coef {get;set;}
    public string repair_price_coef {get;set;}
    public string insurance_price_coef {get;set;}
    public string exchange_price_coef {get;set;}
    public string heal_price_coef {get;set;}
}
