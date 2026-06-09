using _MGGTmod.types.models.Custom;
using _MGGTmod.types.models.EFT.locales;
using _MGGTmod.types.models.EFT.templetes;
using _MGGTmod.types.models.Paths;
using _MGGTmod.types.utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Templates;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Services.Mod;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Constants;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;

namespace _MGGTmod.types.server;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class TemplatesServer(
    ISptLogger<TemplatesServer> logger,
    DatabaseService databaseService,
    LocalesServer localesServer,
    CustomItemService customItemService,
    MGUtils mGUtils
    )
{
    private Templates Templates_ => databaseService.GetTemplates();
    // Item.json
    public Dictionary<MongoId, TemplateItem> GetItems()
    {
        return Templates_.Items;
    }
    public bool IsItemExists(string ItemId)
    {
        return GetItems().ContainsKey(ItemId);
    }
    public string FindItemParentIdById(string ItemId)
    {
        var Items = GetItems();
        if (Items.ContainsKey(ItemId))
        {
            return Items[ItemId].Parent;
        }
        return "null";
    }
    public List<string> FindItemParentsIdById(string ItemId)
    {
        List<string> ParentList = new(){};
        string NowId = ItemId;
        var Items = GetItems();
        while (Items.Keys.Contains(NowId) && Items[NowId].Parent != "")
        {
            string Temp = FindItemParentIdById(NowId);
            if (Temp == "") { break; }
            ParentList.Add(Temp);
            NowId = Temp;
        }
        return ParentList;
    }

    public bool AddItemToDB(TemplateItem item)
    {
        var Items = GetItems();
        var flag = Items.TryAdd(item.Id, item);
        return flag;
    }
    public void AddFilters(Dictionary<string,string> _filterList)
    {
        var itemsDB = GetItems();
        List<string> filterList = new() { "StackSlots", "Slots", "Chambers", "Cartridges", "Grids" };
        foreach (var itemId in itemsDB.Keys)
        {
            var itemProp = itemsDB[itemId].Properties?? null;
            if (itemProp == null) continue;
            var StackSlots = itemProp.StackSlots ?? null;
            var Slots = itemProp.Slots ?? null;
            var Chambers = itemProp.Chambers ?? null;
            var Cartridges = itemProp.Cartridges ?? null;
            var Grids = itemProp.Grids ?? null;
            if (StackSlots != null && StackSlots.Count() > 0)
            {
                foreach (var slot in StackSlots)
                {
                    if (slot.Properties == null || slot.Properties.Filters == null || slot.Properties.Filters.Count() == 0)
                        continue;
                    foreach (var filter in slot.Properties.Filters)
                    {
                        if(filter.Filter == null || filter.Filter.Count == 0) continue;
                        foreach (var fL in _filterList)
                        {
                            if (!filter.Filter.Contains(fL.Value) || filter.Filter.Contains(fL.Key)) continue;
                            filter.Filter.Add(fL.Key);
                        }
                    } 
                }
            }
            
            if (Slots != null && Slots.Count() > 0)
            {
                foreach (var slot in Slots)
                {
                    if (slot.Properties == null || slot.Properties.Filters == null || slot.Properties.Filters.Count() == 0)
                        continue;
                    foreach (var filter in slot.Properties.Filters)
                    {
                        if(filter.Filter == null || filter.Filter.Count == 0) continue;
                        foreach (var fL in _filterList)
                        {
                            if (!filter.Filter.Contains(fL.Value) || filter.Filter.Contains(fL.Key)) continue;
                            filter.Filter.Add(fL.Key);
                        }
                    } 
                }
            }
            
            if (Chambers != null && Chambers.Count() > 0)
            {
                foreach (var chamber in Chambers)
                {
                    if (chamber.Properties == null || chamber.Properties.Filters == null || chamber.Properties.Filters.Count() == 0)
                        continue;
                    foreach (var filter in chamber.Properties.Filters)
                    {
                        if(filter.Filter == null || filter.Filter.Count == 0) continue;
                        foreach (var fL in _filterList)
                        {
                            if (!filter.Filter.Contains(fL.Value) || filter.Filter.Contains(fL.Key)) continue;
                            filter.Filter.Add(fL.Key);
                        }
                    } 
                }
            }

            if (Cartridges != null && Cartridges.Count() > 0)
            {
                foreach (var cartridge in Cartridges)
                {
                    if (cartridge.Properties == null || cartridge.Properties.Filters == null || cartridge.Properties.Filters.Count() == 0)
                        continue;
                    foreach (var filter in cartridge.Properties.Filters)
                    {
                        if(filter.Filter == null || filter.Filter.Count == 0) continue;
                        foreach (var fL in _filterList)
                        {
                            if (!filter.Filter.Contains(fL.Value) || filter.Filter.Contains(fL.Key)) continue;
                            filter.Filter.Add(fL.Key);
                        }
                    } 
                }
            }

            if (Grids != null && Grids.Count() > 0)
            {
                foreach (var grid in Grids)
                {
                    if (grid.Properties == null || grid.Properties.Filters == null || grid.Properties.Filters.Count() == 0)
                        continue;
                    foreach (var filter in grid.Properties.Filters)
                    {
                        if(filter.Filter == null || filter.Filter.Count == 0) continue;
                        foreach (var fL in _filterList)
                        {
                            if (!filter.Filter.Contains(fL.Value) || filter.Filter.Contains(fL.Key)) continue;
                            filter.Filter.Add(fL.Key);
                        }
                    } 
                }
            }
        }
    }
    
    // handbook.json
    public HandbookBase GetHandbook()
    {
        return Templates_.Handbook;
    }
    public string FindHBItemParentId(string ItemId)
    {
        foreach (var item in Templates_.Handbook.Items)
        {
            if (item.Id == ItemId)
            {
                return item.ParentId;
            }
        }
        return "null";
    }
    public void AddHbCategory(HandbookCategory HbCategory)
    {
        if (!MongoId.IsValidMongoId(HbCategory.Id)) return;
        
        if (HbCategory.ParentId == null)
        {
            Templates_.Handbook.Categories.Add(HbCategory);
            return;
        }
        int v = 0;
        foreach (var Categories in Templates_.Handbook.Categories)
        {
            if (Categories.Id == HbCategory.ParentId)
            {
                v = 1;
                break;
            }
        }
        if (v == 0) return;
        Templates_.Handbook.Categories.Add(HbCategory);
    }
    
    // prices.json
    public Dictionary<MongoId, double> GetPrices()
    {
        return databaseService.GetPrices();
    }
    
    
    // profile.json
    public Dictionary<string, ProfileSides> GetProfiles()
    {
        return databaseService.GetProfileTemplates();
    }

    public void AddTraderInitLoyaltyLevel(MongoId Id, int Level = 1)
    {
        var profiles = GetProfiles();
        foreach (var profile in profiles)
        {
            profile.Value.Bear.Trader.InitialLoyaltyLevel.TryAdd(Id, Level);
            profile.Value.Usec.Trader.InitialLoyaltyLevel.TryAdd(Id, Level);
        }
    }

    // traders
    public Dictionary<MongoId, Trader> GetTraders()
    {
        return databaseService.GetTraders();
    }
    
    // quest
    public Dictionary<MongoId, Quest> GetQuests()
    {
        return databaseService.GetQuests();
    }
    
    public void AddCustomItem(NewItemFromCloneDetails item)
    {
        customItemService.CreateItemFromClone(item);
    }
    public void AddCustomItems(List<NewItemFromCloneDetails> itemList)
    {
        foreach(var item in itemList)
        {
            AddCustomItem(item);
        }
    }
    public void AddMGItemsToDB(MGItem item)
    {

        if(!IsItemExists(item.items.cloneId))
        {
            logger.LogWithColor($"MG独立物品id为{item.items.newId}的\"cloneId\"未能在items.json找到，无法添加到游戏中，请检查\"cloneId\"是否正确！", LogTextColor.Cyan);
            return;
        }
        
        /*
        if (FindHBItemParentId(item.items.cloneId) == "null")
        {
            logger.LogWithColor($"MG独立物品id为{item.items.newId}的\"cloneId\"未能在handbook.json中找到，无法添加到游戏中，请检查\"cloneId\"是否正确", LogTextColor.Yellow);
            return;
        }
        */
        if (!mGUtils.IsMongoId(item.items.newId))
        {
            logger.Warning($"MG独立物品id为{item.items.newId}的\"newId\"不符合MongoId格式，建议修改newId，否则游戏内会报错。如果你安装了解除MongoId限制的mod，请忽视此警告消息。");
        }
        var newItemDetails = new NewItemFromCloneDetails()
        {
            FleaPriceRoubles = item.price,
            HandbookParentId = item.HandbookId??"5b47574386f77428ca22b2f4",
            HandbookPriceRoubles = item.price,
            ItemTplToClone = item.items.cloneId,
            Locales = new Dictionary<string, LocaleDetails>()
            {
                {"ch", item.description }
            },
            NewId = item.items.newId,
            OverrideProperties = item.items._props,
            ParentId = FindItemParentIdById(item.items.cloneId),
        };
        AddCustomItem(newItemDetails);  //!!!!!
        var filterList = new Dictionary<string, string>()
        { { item.items.newId, item.items.cloneId} };
        AddFilters(filterList);
    }

    public bool AddCustomTraderItemsToDB(CustomTraderItems item)
    {
        var flag = AddItemToDB(item.item);
        return flag;
    }
    public void AddProfile()
    {

    }
    
}
