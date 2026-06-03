using _MGMod.types.models.EFT.templetes;
using _MGMod.types.models.Paths;
using _MGMod.types.server;
using _MGMod.types.utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Models.Spt.Mod;
using Path = System.IO.Path;

namespace _MGMod.types.services;
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class CustomItemServices(
    ISptLogger<CustomItemServices> logger,
    DatabaseService databaseService,
    LocalesServer localesServer,
    MGUtils mGUtils,
    FileUtil fileUtil,
    JsonUtil jsonUtil,
    TemplatesServer templatesServer,
    TradersServer tradersServer,
    GlobalsServer globalsServer,
    CustomAssortServices customAssortServices
    )
{
    public void Start()
    {
        SuperItemsToMG();
        BroItemsToMG();
        var ItemList = GetAllItems();
        var newItemList = TransferMGItemsStruct(ItemList);
        AddMGItemsToDB(newItemList);
    }

    public Dictionary<string, MGItem> GetAllItems()
    {
        var files = mGUtils.GetFiles(Paths.MGItemDB);
        var ItemList = new Dictionary<string, MGItem>();
        var num = 1;
        foreach (var file in files)
        {
            if (!fileUtil.FileExists(file)) continue;
            var fileName = fileUtil.StripExtension(file);
            var item = jsonUtil.DeserializeFromFile<MGItem>(file);
            ItemList[fileName] = item ?? new MGItem();
        }
        return ItemList;
    }

    public void SuperItemsToMG()
    {
        string[] superItemProp = ["tpl", "items", "handbook"];
        var files = mGUtils.GetFiles(Paths.SuperItemPath);
        foreach(var file in files)
        {
            
            if (!fileUtil.FileExists(file)) continue;
            var fileName = fileUtil.StripExtension(file);
            var item = jsonUtil.DeserializeFromFile<SuperItem>(file);
            List<string> errKey = new();
            foreach(var err in superItemProp)
            {
                if (!mGUtils.HasProp(item, err) ?? false)
                {
                    errKey.Add(err);
                }
            }
            if (errKey.Count > 0)
            {
                logger.LogWithColor($"超模独立物品：{fileName}.json缺少关键属性：{string.Join(", ", errKey)}，请重新检查格式。", LogTextColor.Cyan);
                continue;
            }

            var mgItem = new MGItem
            {
                items = new MGItems
                {
                    newId = item.items._id,
                    cloneId = item.tpl,
                    _props = item.items._props
                },
                price = item.handbook.Price??1,
                description = new LocaleDetails
                {
                    Name = item.items._props.Name?? "unKnown",
                    Description = item.items._props.Description?? "unKnown",
                    ShortName = item.items._props.ShortName?? "unKnown"
                } ,
                toTraderId = "8ef5b2eff000000000000000",
                isSold = true,
                loyal_level = 1,
                assort = item.assort ?? new List<Item>(),
                currency = Money.ROUBLES,
                Buffs = item.Buffs ?? new Dictionary<string, List<Buff>>()
            };
            var handbookItemParentId = item.handbook.ParentId;
            if ( handbookItemParentId != String.Empty || handbookItemParentId != null)
            {
                mgItem.HandbookId = handbookItemParentId;
            }
            else
            {
                mgItem.HandbookId = "5b47574386f77428ca22b2f4";
            }
            string Data = mGUtils.Serialize(mgItem);
            mGUtils.WriteFile(Path.Combine(Paths.MGItemDB, $"{fileName}-Super.json"), Data);
            fileUtil.DeleteFile(file);
        }
    }

    public void BroItemsToMG()
    {
        List<string> broItemProp = new() { "newId", "itemTplToClone", "overrideProperties", "locales" };
        var files = mGUtils.GetFiles(Paths.BrothersItemDB);
        foreach(var file in files)
        {
            if (!fileUtil.FileExists(file)) continue;
            var fileName = fileUtil.StripExtension(file);
            var item = jsonUtil.DeserializeFromFile<BrothersItem>(file);

            List<string> errKey = new();
            foreach (var err in broItemProp)
            {
                if (!mGUtils.HasProp(item, err) ?? false)
                {
                    errKey.Add(err);
                }
            }
            if (errKey.Count > 0)
            {
                logger.LogWithColor($"三兄贵独立物品：{fileName}.json缺少关键属性：{string.Join(", ", errKey)}，请重新检查格式。", LogTextColor.Cyan);
                continue;
            }
            var mgItem = new MGItem
            {
                items = new MGItems
                {
                    newId = item.newId,
                    cloneId = item.itemTplToClone,
                    _props = item.overrideProperties
                },
                price = item.fleaPriceRoubles ?? 1,
                description = item.locales.ch,
                toTraderId = "8ef5b2eff000000000000000",
                isSold = true,
                loyal_level = 1,
                assort = new List<Item>(),
                currency = Money.ROUBLES,
                Buffs = item.Buffs ?? new Dictionary<string, List<Buff>>()
            };
            var handbookItemParentId = templatesServer.FindHBItemParentId(item.itemTplToClone);
            if ( handbookItemParentId != "null")
            {
                mgItem.HandbookId = handbookItemParentId;
            }
            else
            {
                mgItem.HandbookId = "5b47574386f77428ca22b2f4";
            }
            string Data = mGUtils.Serialize(mgItem);
            mGUtils.WriteFile(Path.Combine(Paths.MGItemDB, $"{fileName}-Bro.json"), Data);
            fileUtil.DeleteFile(file);
        }
    }
    
    public void AddMGItemsToDB(Dictionary<string, MGItem> ItemList)
    {
        var count = 0;
        foreach(var it in ItemList.Keys)
        {
            
            var item = ItemList[it];
            var resp = DectectMGItemKey(item);
            
            if (resp.A)
            {
                logger.LogWithColor($"MG独立物品：{it}.json缺少关键属性：{string.Join(", ", resp.B)}", LogTextColor.Yellow);
                continue;
            }
            if (databaseService.GetItems().Keys.Contains(item.items.newId))
            {
                logger.LogWithColor($"MG独立物品：{it}.json的newId已存在于items.json中，请修改newId。", LogTextColor.Yellow);
                continue;
            }
            templatesServer.AddMGItemsToDB(item);
            if (item.isSold ?? false)
            {
                var newAssorts = customAssortServices.CreateCustomItemAssorts(item);
                tradersServer.AddAssortsToTrader(newAssorts);
            }
            if(item.Buffs?.Keys.Count>0)
            {
                globalsServer.AddBuffs(item.Buffs);
            }

            count = count + 1;
        }
        Log($"已添加{count}个独立物品。", LogTextColor.Yellow);
    }

    public Dictionary<string, MGItem> TransferMGItemsStruct(Dictionary<string, MGItem> ItemList)
    {
        List<string> keyName = new() { "Buffs", "toTraderId", "isSold", "loyal_level", "assort", "currency", "HandbookId" };
        List<object?> value = new() { new Dictionary<string, Buff> { }, "8ef5b2eff000000000000000", true, 1, new List<Item> { }, Money.ROUBLES, "5b47574386f77428ca22b2f4" };
        foreach ( var it in ItemList.Keys )
        {
            var item = ItemList[it];
            var resp = DectectMGItemKey(item);
            if (resp.A)
            {
                logger.LogWithColor($"MG独立物品：{it}.json缺少关键属性：{string.Join(", ", resp.B)}", LogTextColor.Yellow);
                continue;
            }

            if (item.Buffs == null) item.Buffs = new();
            if (item.toTraderId == null) item.toTraderId = "8ef5b2eff000000000000000";
            if (item.isSold == null) item.isSold = true;
            if (item.loyal_level == null) item.loyal_level = 1;
            if (item.assort == null) item.assort = new();
            if (item.currency == null) item.currency = Money.ROUBLES;
            if (item.HandbookId == null)
            {
                var HBItemParentId = templatesServer.FindHBItemParentId(item.items.cloneId);
                if (HBItemParentId != "null") item.HandbookId = HBItemParentId;
                else item.HandbookId = "5b47574386f77428ca22b2f4";
            }
            string Data = mGUtils.Serialize<MGItem>(item);
            mGUtils.DeleteFile(Path.Combine(Paths.MGItemDB, $"{it}.json"));
            mGUtils.WriteFile(Path.Combine(Paths.MGItemDB, $"{it}.json"), Data);
        }
        return ItemList;
    }

    public (MGItem A, bool B)? CreatNewKey(MGItem item, string key, object value)
    {
        if (mGUtils.HasProp(item, key) ?? false)
        {
            return (item, false);
        }
        bool setSuccess = mGUtils.SetPropValue(item,key,value);
        return (item, setSuccess);
    }
    
    public (bool A, List<string> B) DectectMGItemKey(MGItem item)
    {
        string[] baseProperty = ["items", "price", "description"];
        List<string> errKey = new();
        foreach (var key in baseProperty)
        {
            if(!mGUtils.HasProp(item, key)??true)
            {
                errKey.Add(key);
            }
        }
        return (errKey.Count>0, errKey);
    }

    private void Log(string data, LogTextColor textColor)
    {
        mGUtils.Log("独立物品", data, textColor);
    }
}
