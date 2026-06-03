using _MGGTmod.types.models.EFT;
using _MGGTmod.types.models.EFT.locales;
using _MGGTmod.types.models.EFT.locations;
using _MGGTmod.types.models.EFT.templetes;
using _MGGTmod.types.models.EFT.traders;
using _MGGTmod.types.models.Paths;
using _MGGTmod.types.server;
using _MGGTmod.types.utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Loaders;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Routers;
using Path = System.IO.Path;

namespace _MGGTmod.types.services;
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class CustomTraderServices
{
    private MGUtils mGUtils;
    private ISptLogger<CustomTraderServices> logger;
    private ImageRouter  imageRouter;
    private LocalesServer localesServer;
    private ConfigsServer configsServer;
    private TemplatesServer templatesServer;
    private GlobalsServer globalsServer;
    private LocationsServer locationsServer;
    
    public CustomTraderServices(
        MGUtils _mGUtils,
        ISptLogger<CustomTraderServices> _logger,
        ImageRouter _imageRouter,
        LocalesServer _localesServer,
        ConfigsServer _configsServer,
        TemplatesServer _templatesServer,
        GlobalsServer _globalsServer,
        LocationsServer _locationsServer
    )
    {
        mGUtils = _mGUtils;
        logger = _logger;
        imageRouter = _imageRouter;
        localesServer = _localesServer;
        configsServer = _configsServer;
        templatesServer = _templatesServer;
        globalsServer = _globalsServer;
        locationsServer = _locationsServer;
    }
    
    
    public void Start()
    {
        var Bundles = AddTraders();
        AddBundles(Bundles);
    }

    public BundleManifest AddTraders()
    {
        var TradersDirectories = mGUtils.GetDirectories(Paths.Traders);
        BundleManifest Bundles = new BundleManifest
        {
            Manifest = []
        };
        foreach (var traderPath in TradersDirectories)
        {
            bool addFlag = AddTraderBaseToDB(traderPath);
            if (!addFlag) continue;
            AddImageToDB(traderPath);
            AddTraderItemsToDB(traderPath);
            AddTraderLocalesToDB(traderPath);
            AddTraderLocationToDB(traderPath);
            AddTraderTemplatesToDB(traderPath);
            AddTraderGlobalsToDB(traderPath);
            Log($"商人【{Path.GetFileName(traderPath)}】已添加。", LogTextColor.Yellow);
            BundleManifest bundles = mGUtils.GetJsonDataFromFile<BundleManifest>(new PathType
            {
                FileName = TraderPathsType.TraderBundles,
                Path = traderPath,
            }, false);
            if(bundles.Manifest.Count > 0) Bundles.Manifest.AddRange(bundles.Manifest);
        }
        return Bundles;
    }

    public void AddBundles(BundleManifest Bundles)
    {
        BundleManifest mainBundles = mGUtils.GetJsonDataFromFile<BundleManifest>(Paths.BundlesJson);
        mainBundles.Manifest.Clear();
        if (Bundles.Manifest.Count > 0)
        {
            mainBundles.Manifest.AddRange(Bundles.Manifest);
        }
        mGUtils.DeleteFile(Paths.BundlesJson.FileName);
        mGUtils.WriteFile(Paths.BundlesJson.FileName, mGUtils.Serialize(mainBundles));
    }

    public bool AddTraderBaseToDB(string traderPath)
    {
        CustomTraderInfo traderInfo = mGUtils.GetJsonDataFromFile<CustomTraderInfo>(new PathType
        {
            FileName = TraderPathsType.TraderInfo,
            Path = traderPath,
        }, false);
        var returnFlag = 0;
        if (traderInfo == default)
        {
            // 2026.01.19 23:14 进度于此
            Log($"商人{Path.GetFileName(traderPath)}不存在配置文件\"traderInfo.json\"，请检查商人文件完整性。",LogTextColor.Cyan);
            returnFlag = returnFlag + 1;
        }
        
        var Traders = templatesServer.GetTraders();
        if (Traders.ContainsKey(traderInfo._id))
        {
            Log($"商人{Path.GetFileName(traderPath)}的Id:{traderInfo._id}已存在于游戏中,请修改。",LogTextColor.Cyan);
            returnFlag = returnFlag + 1;
        }
        
        if (returnFlag != 0) return false;
        if (!MongoId.IsValidMongoId(traderInfo._id)) 
            Log($"商人{traderInfo.name}的Id:{traderInfo._id}不符合MongoId格式，请酌情修改。【如果你安装了无视MongoId限制的Mod，可忽视本条消息】",LogTextColor.Cyan);
        
        if (!traderInfo.enable) return false;

        TraderBase traderBase = mGUtils.GetJsonDataFromFile<TraderBase>(new PathType
        {
            FileName = "base.json",
            Path = Paths.TraderDB,
        });
        
        // base.json
        traderBase.Id = traderInfo._id;
        traderBase.Name = traderInfo.locales.FullName;
        traderBase.Surname = traderInfo.locales.FirstName;
        traderBase.Nickname = traderInfo.locales.Nickname;
        traderBase.Location = traderInfo.locales.Location;
        traderBase.Insurance.Availability = traderInfo.insurance?.enable;
        traderBase.Insurance.MinPayment = traderInfo.insurance?.pay;
        traderBase.Insurance.MinReturnHour =  traderInfo.insurance?.minreturntime;
        traderBase.Insurance.MaxReturnHour = traderInfo.insurance?.maxreturntime;
        traderBase.Insurance.MaxStorageTime = traderInfo.insurance?.storageTime;
        traderBase.Repair.Availability = traderInfo.repair?.enable;
        traderBase.Repair.CurrencyCoefficient = traderInfo.repair?.coefficient;
        traderBase.Repair.Quality = traderInfo.repair?.quality;
        traderBase.Medic = traderInfo.medic;
        traderBase.LoyaltyLevels = traderInfo.loyaltyLevels?.range;
        traderBase.Discount = traderInfo.discount;
        traderBase.UnlockedByDefault = traderInfo.unlockedDefault;
        
        Trader newTrader = new Trader
        {
            Assort = new TraderAssort
            {
                Items = new(),
                BarterScheme = new(),
                LoyalLevelItems = new()
            },
            Base = traderBase,
            Dialogue = new Dictionary<string, List<string>?>
            {
                ["insuranceStart"] =[],
                ["insuranceFound"] = [],
                ["insuranceFailed"] = [],
                ["insuranceExpired"] = [],
                ["insuranceComplete"] = [],
                ["insuranceFailedLabs"] = []
                
            },
            QuestAssort = new Dictionary<string, Dictionary<MongoId, MongoId>>
            {
                ["started"] = new (){},
                ["success"] = new (){},
                ["fail"] = new (){},
            }
        };
        
        // dialogue.json
        foreach (var dialogue in newTrader.Dialogue)
        {
            if (traderInfo.insurance.Message.TryGetValue(dialogue.Key, out var message))
            {
                dialogue.Value?.AddRange(message);
            }
        }
        // questassort.json  Dictionary<string, Dictionary<MongoId, MongoId>>
        Dictionary<string, Dictionary<string, string>> traderQuestAssort = mGUtils.GetJsonDataFromFile<Dictionary<string, Dictionary<string, string>>>(new PathType
        {
            FileName = "questassort.json",
            Path = Path.Combine(traderPath,TraderPathsType.TraderDataPath),
        }, false);
        // assort.json
        TraderAssortStringId traderAssortStringId = mGUtils.GetJsonDataFromFile<TraderAssortStringId>(new PathType
        {
            FileName = "assort.json",
            Path = Path.Combine(traderPath,TraderPathsType.TraderDataPath),
        }, false);
        if (traderAssortStringId.items.Count > 0)
        {
            foreach (var item in traderAssortStringId.items)
            {
                if(mGUtils.IsMongoId(item._id)) continue;
                MongoId newId = new MongoId();
                traderAssortStringId = mGUtils.ReplaceKey(traderAssortStringId, item._id, newId);
                traderQuestAssort = mGUtils.ReplaceKey(traderQuestAssort, item._id, newId);
            }
            TraderAssort traderAssort = mGUtils.Deserialize<TraderAssort>(mGUtils.Serialize(traderAssortStringId));
            newTrader.Assort.Items.AddRange(traderAssort.Items);
            foreach (var item in traderAssort.BarterScheme)
            {
                newTrader.Assort.BarterScheme.TryAdd(item.Key, item.Value);
            }
            foreach (var item in traderAssort.LoyalLevelItems)
            {
                newTrader.Assort.LoyalLevelItems.TryAdd(item.Key, item.Value);
            }

            foreach (var key in traderQuestAssort)
            {
                foreach (var key2 in key.Value)
                {
                    newTrader.QuestAssort[key.Key].TryAdd(key2.Key, key2.Value);
                }
            }
        }

        // image
        string traderImage = $"{traderInfo._id}.jpg";
        string imagePath = Path.Combine(traderPath,traderImage);
        if (mGUtils.FileExists(imagePath))
        {
            newTrader.Base.Avatar = newTrader.Base.Avatar.Replace("000000000000000000000000.jpg", traderImage);
            imageRouter.AddRoute(newTrader.Base.Avatar.Replace(".jpg",""), imagePath);
        }
        else Log($"{traderInfo.name}：混蛋，你把我的头像放哪了！快还给我！", LogTextColor.Cyan);

        if (!Traders.TryAdd(traderInfo._id, newTrader))
        {
            Log($"{traderInfo.name}：添加失败，发生了未知错误。", LogTextColor.Cyan);
            return false;
        }
        
        // locales
        localesServer.AddTraderInfo(new _MGGTmod.types.models.EFT.locales.TraderInfo
        {
            Id = traderInfo._id,
            Desc = traderInfo.locales
        });
        
        // insurance.json
        if (traderInfo.insurance.enable) configsServer.AddTraderReturnChance(traderInfo._id, traderInfo.insurance.chance);
        // quest.json
        configsServer.AddRepeatableQuestTraderWhitelist(traderInfo._id, traderInfo.locales.Nickname);
        // ragfair.json
        configsServer.AddTraderRagfair(traderInfo._id);
        // traders.json
        configsServer.SetTradersUpdateTime(traderInfo.updateTime.Min,traderInfo.updateTime.Max, traderInfo._id, traderInfo.locales.Nickname);

        templatesServer.AddTraderInitLoyaltyLevel(traderInfo._id);

        return true;
    }

    public void AddImageToDB(string traderPath)
    {
        // quests
        var questsPath =  Path.Combine(traderPath,TraderPathsType.TraderQuestImagesPath);
        if (mGUtils.DirectoryExists(questsPath,false))
        {
            var iconList = mGUtils.GetFiles(questsPath);
            foreach (var iconPath in iconList)
            {
                imageRouter.AddRoute($"/files/quest/icon/{mGUtils.StripExtension(iconPath)}", $"{Path.Combine(questsPath,iconPath)}");
            }
        }
        
    }

    public void AddTraderItemsToDB(string traderPath)
    {
        Dictionary<string, string> filterList = new();
        string TraderItemsPath = Path.Combine(traderPath, TraderPathsType.TraderItemsPath);
        var itemList = mGUtils.GetFiles(TraderItemsPath);
        foreach (var itemFilePath in itemList)
        {
            CustomTraderItems traderItem = mGUtils.GetJsonDataFromFile<CustomTraderItems>(new()
            {
                FileName = Path.GetFileName(itemFilePath),
                Path = TraderItemsPath,
            }, false);
            if (templatesServer.IsItemExists(traderItem.item.Id))
            {
                Log($"【警告】独立商人物品【id:{traderItem.item.Id}】已存在，请酌情修改id，本次不执行添加操作。", LogTextColor.Cyan);
            }

            if (!String.IsNullOrEmpty(traderItem.origin) && mGUtils.IsMongoId(traderItem.origin))
                filterList.TryAdd(traderItem.item.Id, traderItem.origin);
            var flag = templatesServer.AddCustomTraderItemsToDB(traderItem);
            if(!flag) Log($"【警告】独立商人物品【id:{traderItem.item.Id}】添加失败，请检查物品文件是否正确。", LogTextColor.Cyan);
        }
        templatesServer.AddFilters(filterList);
    }

    public void AddTraderLocalesToDB(string traderPath)
    {
        // itemsdescription.json
        Dictionary<MongoId, ItemsDesc> itemsDescs = mGUtils.GetJsonDataFromFile<Dictionary<MongoId, ItemsDesc>>(new PathType
        {
            FileName = TraderPathsType.TraderItemDesc.FileName,
            Path = Path.Combine(traderPath,TraderPathsType.TraderItemDesc.Path),
        }, false);
        foreach (var item in itemsDescs)
        {
            localesServer.AddItemInfo(new ItemsInfo
            {
                Id = item.Key,
                Desc = item.Value
            });
        }
        
        // mail.json
        Dictionary<MongoId, QuestDesc> mails = mGUtils.GetJsonDataFromFile<Dictionary<MongoId, QuestDesc>>(new PathType
        {
            FileName = TraderPathsType.TraderMail.FileName,
            Path = Path.Combine(traderPath, TraderPathsType.TraderMail.Path),
        }, false);
        foreach (var quest in mails)
        {
            localesServer.AddQuestInfo(new QuestInfo
            {
                Id = quest.Key,
                Desc = quest.Value
            });
        }
    }

    public void AddTraderLocationToDB(string traderPath)
    { 
        
    }

    public void AddTraderTemplatesToDB(string traderPath)
    {
        // handbook.json
        HandbookBase handbookBase = templatesServer.GetHandbook();
        List<HandbookItem> items = mGUtils.GetJsonDataFromFile<List<HandbookItem>>(new PathType
        {
            FileName = TraderPathsType.TraderHandbook.FileName,
            Path = Path.Combine(traderPath, TraderPathsType.TraderHandbook.Path),
        }, false);
        handbookBase.Items.AddRange(items);
        
        // quests.json
        Dictionary<MongoId, Quest> quests = templatesServer.GetQuests();
        Dictionary<MongoId, Quest> mGQuests = mGUtils.GetJsonDataFromFile<Dictionary<MongoId, Quest>>(new PathType
        {
            FileName = TraderPathsType.TraderQuests.FileName,
            Path = Path.Combine(traderPath, TraderPathsType.TraderQuests.Path),
        }, false);
        foreach (var quest in mGQuests)
        {
            quests.TryAdd(quest.Key, quest.Value);
        }

    }

    public void AddTraderGlobalsToDB(string traderPath)
    {
        CustomGlobals mGGlobals = mGUtils.GetJsonDataFromFile<CustomGlobals>(new PathType
        {
            FileName = TraderPathsType.TraderGlobals,
            Path = traderPath,
        }, false);
        Globals globals = globalsServer.GetGlobals();
        
        // ItemPreset
        if (mGGlobals.ItemPresets != null)
        {
            foreach (var itemPreset in mGGlobals.ItemPresets)
            {
                globals.ItemPresets.Add(itemPreset.Key, itemPreset.Value);
            }
        }
        
        // Buffs
        if (mGGlobals.Buffs != null)
        {
            if (mGGlobals.Buffs?.Count > 0)
            {
                globalsServer.AddBuffs(mGGlobals.Buffs);
            }
        }
        
        
    }

    public void Log(string data, LogTextColor textColor)
    {
        mGUtils.Log_GT("商人系统", data, textColor);
    }
}