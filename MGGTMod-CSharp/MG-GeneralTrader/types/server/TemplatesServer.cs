using _MGMod.types.models.Custom;
using _MGMod.types.models.EFT.locales;
using _MGMod.types.models.EFT.templetes;
using _MGMod.types.models.Paths;
using _MGMod.types.utils;
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

namespace _MGMod.types.server;

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
                    var filterListObj = slot.Properties.Filters.ElementAtOrDefault(0);
                    if (filterListObj == null || filterListObj.Filter == null)
                        continue;
                    var filter = filterListObj.Filter;
                    foreach (var fL in _filterList)
                    {
                        if (!filter.Contains(fL.Value) || filter.Contains(fL.Key)) continue;
                        filter.Add(fL.Key);
                    }
                }
            }
            
            if (Slots != null && Slots.Count() > 0)
            {
                foreach (var slot in Slots)
                {
                    if (slot.Properties == null || slot.Properties.Filters == null || slot.Properties.Filters.Count() == 0)
                        continue;
                    var filterListObj = slot.Properties.Filters.ElementAtOrDefault(0);
                    if (filterListObj == null || filterListObj.Filter == null)
                        continue;
                    var filter = filterListObj.Filter;
                    foreach (var fL in _filterList)
                    {
                        if (!filter.Contains(fL.Value) || filter.Contains(fL.Key)) continue;
                        filter.Add(fL.Key);
                    }
                }
            }
            
            if (Chambers != null && Chambers.Count() > 0)
            {
                foreach (var chamber in Chambers)
                {
                    if (chamber.Properties == null || chamber.Properties.Filters == null || chamber.Properties.Filters.Count() == 0)
                        continue;
                    var filterListObj = chamber.Properties.Filters.ElementAtOrDefault(0);   
                    if (filterListObj == null || filterListObj.Filter == null)
                        continue;
                    var filter = filterListObj.Filter;
                    foreach (var fL in _filterList)
                    {
                        if (!filter.Contains(fL.Value) || filter.Contains(fL.Key)) continue;
                        filter.Add(fL.Key);
                    }
                }
            }

            if (Cartridges != null && Cartridges.Count() > 0)
            {
                foreach (var cartridge in Cartridges)
                {
                    if (cartridge.Properties == null || cartridge.Properties.Filters == null || cartridge.Properties.Filters.Count() == 0)
                        continue;
                    var filterListObj = cartridge.Properties.Filters.ElementAtOrDefault(0);
                    if (filterListObj == null || filterListObj.Filter == null)
                        continue;
                    var filter = filterListObj.Filter;
                    foreach (var fL in _filterList)
                    {
                        if (!filter.Contains(fL.Value) || filter.Contains(fL.Key)) continue;
                        filter.Add(fL.Key);
                    }
                }
            }

            if (Grids != null && Grids.Count() > 0)
            {
                foreach (var grid in Grids)
                {
                    if (grid.Properties == null || grid.Properties.Filters == null || grid.Properties.Filters.Count() == 0)
                        continue;
                    var filterListObj = grid.Properties.Filters.ElementAtOrDefault(0);
                    if (filterListObj == null || filterListObj.Filter == null)
                        continue;
                    var filter = filterListObj.Filter;
                    foreach (var fL in _filterList)
                    {
                        if (!filter.Contains(fL.Value) || filter.Contains(fL.Key)) continue;
                        filter.Add(fL.Key);
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

    public void MGmodTemplates(MGModConfig_Templates TemplatesSetting)
    {
        MGmodItems(TemplatesSetting);
        MGmodQuests(TemplatesSetting);
    }

    private void MGmodItems(MGModConfig_Templates TemplatesSetting)
    {
        string[] AmmoBlacklist = [
            "5656eb674bdc2d35148b457c",
            "5ede474b0c226a66f5402622",
            "5ede475b549eed7c6d5c18fb",
            "5ede4739e0350d05467f73e8",
            "5ede47405b097655935d7d16",
            "5ede475339ee016e8c534742",
            "5f0c892565703e5c461894e9",
            "62389aaba63f32501b1b444f",
            "62389ba9a63f32501b1b4451",
            "62389bc9423ed1685422dc57",
            "62389be94d5d474bf712e709",
            "635267f063651329f75a4ee8"
        ];
        var ContainerExpand = TemplatesSetting.ContainerExpand;
        string[] MedcParent = [ "5448f39d4bdc2d0a728b4568", "5448f3a14bdc2d27728b4569", "5448f3a64bdc2d60728b456a", "5448f3ac4bdc2dce718b4569" ];
        string[] WeaponFilter = [ "FirstPrimaryWeapon", "SecondPrimaryWeapon", "Holster" ];
        HashSet<MongoId> ArmorVestList = new HashSet<MongoId> { "5448e5284bdc2dcb718b4567", "5448e54d4bdc2dcc718b4568", "57bef4c42459772e8d35a53b" };
        var ItemDB = GetItems();
        foreach (var Item in ItemDB)
        {
            string ItemId = Item.Value.Id;
            string ItemParent = Item.Value.Parent;
            var ItemProps = Item.Value.Properties;
            if (ItemParent == "") continue;
            if (ItemProps == null) continue;
            // 物品全检视 Examined
            if (TemplatesSetting.Examined)
            {
                ItemProps.ExaminedByDefault = true;
            }
            // 武器栏可放全部武器 WeaponFilter
            if (ItemId == "55d7217a4bdc2d86028b456d" && TemplatesSetting.WeaponFilter)
            {
                foreach (var item in ItemProps.Slots)
                {
                    if (WeaponFilter.Contains(item.Name))
                    {
                        item.Properties.Filters.ElementAtOrDefault(0).Filter = ["5422acb9af1c889c16000029"];
                    }
                    else if (item.Name == "Scabbard")
                    {
                        item.Properties.Filters.ElementAtOrDefault(0).Filter = ["5422acb9af1c889c16000029", "5447e1d04bdc2dff2f8b4567"];
                    }
                }
            }
            // 子弹
            if (ItemParent == "5485a8684bdc2da71d8b4567")
            {
                // 功能：子弹堆叠 AmmoStack
                if (TemplatesSetting.AmmoStack != 1)
                {
                    // 功能：子弹堆叠 AmmoStack
                    ItemProps.StackMaxSize *= TemplatesSetting.AmmoStack;
                    int StackMaxRandom = ItemProps.StackMaxSize??60;
                    if (TemplatesSetting.AmmoStack > 5)
                    {
                        ItemProps.Weight = 0;
                        StackMaxRandom = 300;
                    }
                    if (TemplatesSetting.AmmoStack > 10)
                    {
                        StackMaxRandom = 600;
                    }
                    if (AmmoBlacklist.Contains(ItemId))
                    {
                        ItemProps.StackMinRandom = 1;
                        ItemProps.StackMaxRandom = 20;
                    } else
                    {
                        ItemProps.StackMinRandom = 30;
                        ItemProps.StackMaxRandom = StackMaxRandom;
                    }

                }
                // 功能：子弹数据 AmmoInfo
                if (TemplatesSetting.AmmoInfo)
                {
                    string retStr_ammo = "<color=#00cccc><b>肉伤：" + ItemProps.Damage + "     甲伤：" + ItemProps.ArmorDamage + "     穿甲：" + ItemProps.PenetrationPower + "     穿透率：" + ItemProps.PenetrationChanceObstacle + "     跳弹率：" + ItemProps.RicochetChance + "     碎弹率：" + ItemProps.FragmentationChance + "</b></color>\r\n";
                    string newDesc = retStr_ammo + localesServer.GetInfoByKey(ItemId + " Description");
                    localesServer.SetInfo(new GeneralInfo
                    {
                        Id = ItemId + " Description",
                        Desc = newDesc,
                    });
                }
            }
            // 容器扩容 ContainerExpand
            if (ContainerExpand.ContainsKey(ItemId))
            {
                // 容器扩容
                if (ContainerExpand[ItemId].enable)
                {
                    ItemProps.Grids.ElementAtOrDefault(0).Properties.CellsH = ContainerExpand[ItemId].cellsH;
                    ItemProps.Grids.ElementAtOrDefault(0).Properties.CellsV = ContainerExpand[ItemId].cellsV;
                }
                // 容器兼容
                if (ContainerExpand[ItemId].Filter)
                {
                    ItemProps.Grids.ElementAtOrDefault(0).Properties.Filters = [
                        new GridFilter{
                            Filter = ["54009119af1c881c07000029"],
                            ExcludedFilter = []
                        }
                        ];
                }
                // 无负重
                if (ContainerExpand[ItemId].Weight)
                {
                    ItemProps.Weight = 0;
                }
            }
            // 保险箱 Safes
            if ( ItemParent == "5448bf274bdc2dfc2f8b456a")
            {
                // 容量格子调整为：宽6高8
                if (TemplatesSetting.Safes.SizeExpand)
                {
                    ItemProps.Grids.ElementAtOrDefault(0).Properties.CellsH = 6;
                    ItemProps.Grids.ElementAtOrDefault(0).Properties.CellsV = 8;
                }
                // 去除安全箱物品存放限制
                if (TemplatesSetting.Safes.Filter)
                {
                    ItemProps.Grids.ElementAtOrDefault(0).Properties.Filters = [
                        new GridFilter{ 
                            Filter = ["54009119af1c881c07000029"], 
                            ExcludedFilter = []
                            },
                        new GridFilter{ 
                            Filter = ["54009119af1c881c07000029"], 
                            ExcludedFilter = []
                            }
                        ];
                }
                // 安全箱重量(可以实现负重)
                if (TemplatesSetting.Safes.NoWeight)
                {
                    ItemProps.Weight = -20;
                }
            }
            // 钱堆叠 MoneyStack
            if (ItemParent == "543be5dd4bdc2deb348b4569" && TemplatesSetting.MoneyStack != 1)
            {
                // 每个堆叠最大数量
                ItemProps.StackMaxSize *= TemplatesSetting.MoneyStack;
                ItemProps.Weight = 0;
                // 每个堆叠的最小和最大随机数量(影响战局内刷到的数量)
                if (ItemId == "5449016a4bdc2d6f028b456f")
                {
                    ItemProps.StackMinRandom = 1000;
                    ItemProps.StackMaxRandom = 10000;
                }
                if (ItemId == "569668774bdc2da2298b4568")
                {
                    ItemProps.StackMinRandom = 100;
                    ItemProps.StackMaxRandom = 500;
                }
                if (ItemId == "5696686a4bdc2da3298b456a")
                {
                    ItemProps.StackMinRandom = 100;
                    ItemProps.StackMaxRandom = 500;
                }
            }
            // 背包、盲盒 Backpack
            if (ItemParent == "5448e53e4bdc2d60728b4567")
            {
                // 去除物品限制
                if (TemplatesSetting.Backpack.Filter)
                {
                    ItemProps.Grids.ElementAtOrDefault(0).Properties.Filters = [
                        new GridFilter{ 
                            Filter=["54009119af1c881c07000029"], 
                            ExcludedFilter=[]
                        }
                        ];
                }
                // 背包折叠
                if (TemplatesSetting.Backpack.SmallSize)
                {
                    ItemProps.Width = ItemProps.Width / 2;
                    ItemProps.Height = ItemProps.Height / 2;
                }

                if (TemplatesSetting.Backpack.NoBuff)
                {
                    // 速度惩罚
                    ItemProps.SpeedPenaltyPercent = 0;
                    // 转向惩罚
                    ItemProps.MousePenalty = 0;
                    // 武器人机工效惩罚
                    ItemProps.WeaponErgonomicPenalty = 0;
                }
                // 无负重
                if (TemplatesSetting.Backpack.NoWeight)
                {
                    ItemProps.Weight = 0;
                }
            }
            // 弹挂修改 护甲修改 Armor
            if (ArmorVestList.Contains(ItemParent))
            {
                if (TemplatesSetting.Armor.Filter)
                {
                    // 去除护甲穿戴冲突
                    ItemProps.BlocksArmorVest = false;
                }
                if (TemplatesSetting.Armor.NoBuff)
                {
                    // 速度惩罚
                    ItemProps.SpeedPenaltyPercent = 0;
                    // 转向惩罚
                    ItemProps.MousePenalty = 0;
                    // 武器人机工效惩罚
                    ItemProps.WeaponErgonomicPenalty = 0;
                }
                // 重量
                if (TemplatesSetting.Armor.NoWeight)
                {
                    ItemProps.Weight = 0;
                }

            }
            // 防护装备耐久
            if (ItemParent == "644120aa86ffbe10ee032b6f"
                || ItemParent == "65649eb40bf0ed77b8044453")
            {
                if (TemplatesSetting.EquipmentPlate.Durability != 1)
                {
                    ItemProps.Durability = ItemProps.Durability * TemplatesSetting.EquipmentPlate.Durability;
                    ItemProps.MaxDurability = ItemProps.MaxDurability * TemplatesSetting.EquipmentPlate.Durability;
                }
                if (TemplatesSetting.EquipmentPlate.NoBuff)
                {
                    // 速度惩罚
                    ItemProps.SpeedPenaltyPercent = 0;
                    // 转向惩罚
                    ItemProps.MousePenalty = 0;
                    // 武器人机工效惩罚
                    ItemProps.WeaponErgonomicPenalty = 0;
                }
                // 重量
                if (TemplatesSetting.EquipmentPlate.NoWeight)
                {
                    ItemProps.Weight = 0;
                }
                
            }
            // 头盔修改 Helmet
            if (ItemParent == "5a341c4086f77401f2541505")
            {
                if (TemplatesSetting.Helmet.Filter)
                {
                    // 去除耳机佩戴冲突
                    ItemProps.BlocksEarpiece = false;
                    // 去除眼睛佩戴冲突
                    ItemProps.BlocksEyewear = false;
                    // 去除面罩佩戴冲突
                    ItemProps.BlocksFaceCover = false;
                    // 去除所有冲突物品
                    ItemProps.ConflictingItems.Clear();
                }
                if (TemplatesSetting.Helmet.NoBuff)
                {
                    // 速度惩罚
                    ItemProps.SpeedPenaltyPercent = 0;
                    // 转向惩罚
                    ItemProps.MousePenalty = 0;
                    // 武器人机工效惩罚
                    ItemProps.WeaponErgonomicPenalty = 0;
                }
                if (TemplatesSetting.Helmet.NoWeight)
                {
                    // 重量
                    ItemProps.Weight = 0;
                }
            }
            // 钥匙和卡无限使用次数 KeysDurability
            if (ItemParent == "5c164d2286f774194c5e69fa"
                || ItemParent == "5c99f98d86f7745c314214b3")
            {
                if (TemplatesSetting.KeysDurability)
                {
                    ItemProps.MaximumNumberOfUsage = 0;
                }
            }
            // 医疗物品耐久调整 MedcDurability
            if (MedcParent.Contains(ItemParent)
                && TemplatesSetting.MedcDurability != 1)
            {
                if (ItemProps.MaxHpResource == 0)
                {
                    ItemProps.MaxHpResource = 1;
                }
                ItemProps.MaxHpResource *= TemplatesSetting.MedcDurability;
                ItemProps.HpResourceRate *= TemplatesSetting.MedcDurability;
            }
            // 武器无故障 WeaponNoLost
            if (FindItemParentsIdById(ItemId).Contains("5422acb9af1c889c16000029"))
            {
                if (ItemProps.BaseMalfunctionChance > 0 && TemplatesSetting.WeaponNoLost)
                {
                    ItemProps.BaseMalfunctionChance = 0;
                    //ItemProps.AllowFeed = false;
                    ItemProps.AllowJam = false;
                    ItemProps.AllowMisfire = true;
                    ItemProps.AllowOverheat = false;
                    ItemProps.AllowSlide = false;
                }
                // 武器维修无损耗 WeaponRepairPerfect
                if (TemplatesSetting.WeaponRepairPerfect)
                {
                    ItemProps.MaxRepairDegradation = 0;     //商人
                    ItemProps.MaxRepairKitDegradation = 0;  //维修包
                }
                /*
                // 武器不消耗耐久
                if (TemplatesSetting.DurabilityBurnModificator)
                {
                    ItemProps.DurabilityBurnModificator = 0;
                }
                */
            }
            //弹匣容量 MagazineCapacity
            if (ItemParent == "5448bc234bdc2d3c308b4569" && ItemProps.Cartridges != null
                && TemplatesSetting.MagazineCapacity != 1)
            {
                foreach (var Cartridge in ItemProps.Cartridges)
                {
                    Cartridge.MaxCount *=  TemplatesSetting.MagazineCapacity;
                }
            }
            // 优化 T7、夜视仪
            if (ItemParent == "5a2c3a9486f774688b05e574"
                && TemplatesSetting.T7ThermalImaging)
            {
                var T7Json = mGUtils.GetJsonDataFromFile<TemplateItem>(Paths.T7Json);
                ItemProps = mGUtils.AssignNonNullProps(T7Json.Properties, ItemProps);
            }
            if (ItemParent == "5d21f59b6dbe99052b54ef83"
                && TemplatesSetting.T7ThermalImaging)
            {
                var GPNVGJson = mGUtils.GetJsonDataFromFile<TemplateItem>(Paths.GPNVGJson);
                ItemProps = mGUtils.AssignNonNullProps(GPNVGJson.Properties, ItemProps);
            }
        }
    }
    private void MGmodQuests(MGModConfig_Templates TemplatesSetting)
    {
        // 功能：任务免费重置 ResetFree
        if (TemplatesSetting.ResetFree)
        {
            var RQT = Templates_.RepeatableQuests.Templates;
            RQT.Elimination.ChangeCost[0].Count = 0;
            RQT.Completion.ChangeCost[0].Count = 0;
            RQT.Exploration.ChangeCost[0].Count = 0;
            RQT.Pickup.ChangeCost[0].Count = 0;
        }
        // 功能：任务优化 QuestOptimize
        if (TemplatesSetting.QuestOptimize)
        {
            var Quest = Templates_.Quests;
            foreach (var qusetId in Quest.Keys)
            {
                var AForFinish = Quest[qusetId].Conditions.AvailableForFinish;
                foreach (var Finish in AForFinish)
                {
                    Finish.Value = 1;
                }
                var AForStart = Quest[qusetId].Conditions.AvailableForStart;
                foreach (var Start in AForStart)
                {
                    Start.AvailableAfter = 0;
                }
            }
        }
    }

}
