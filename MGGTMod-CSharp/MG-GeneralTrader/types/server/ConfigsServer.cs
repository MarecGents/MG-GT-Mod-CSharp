using _MGMod.types.models.Custom;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;

namespace _MGMod.types.server;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]

public class ConfigsServer(
    ConfigServer configServer,
    ISptLogger<ConfigsServer> logger
    )
{
    private AirdropConfig Airdrop => configServer.GetConfig<AirdropConfig>();
    private BackupConfig Backup => configServer.GetConfig<BackupConfig>();
    private BotConfig Bot => configServer.GetConfig<BotConfig>();
    private BtrDeliveryConfig BtrDelivery => configServer.GetConfig<BtrDeliveryConfig>();
    private CoreConfig Core => configServer.GetConfig<CoreConfig>();
    private GiftsConfig Gifts => configServer.GetConfig<GiftsConfig>();
    private HealthConfig Health => configServer.GetConfig<HealthConfig>();
    private HideoutConfig Hideout => configServer.GetConfig<HideoutConfig>();
    private HttpConfig Http => configServer.GetConfig<HttpConfig>();
    private InRaidConfig InRaid => configServer.GetConfig<InRaidConfig>();
    private InsuranceConfig Insurance => configServer.GetConfig<InsuranceConfig>();
    private InventoryConfig Inventory => configServer.GetConfig<InventoryConfig>();
    private ItemConfig Item => configServer.GetConfig<ItemConfig>();
    private LocaleConfig Locale => configServer.GetConfig<LocaleConfig>();
    private LocationConfig Location => configServer.GetConfig<LocationConfig>();
    private LootConfig Loot => configServer.GetConfig<LootConfig>();
    private LostOnDeathConfig LostOnDeath => configServer.GetConfig<LostOnDeathConfig>();
    private MatchConfig Match => configServer.GetConfig<MatchConfig>();
    private PlayerScavConfig PlayerScav => configServer.GetConfig<PlayerScavConfig>();
    private PmcChatResponse PmcChatResponse => configServer.GetConfig<PmcChatResponse>();
    private PmcConfig Pmc => configServer.GetConfig<PmcConfig>();
    private QuestConfig Quest => configServer.GetConfig<QuestConfig>();
    private RagfairConfig Ragfair => configServer.GetConfig<RagfairConfig>();
    private RepairConfig Repair => configServer.GetConfig<RepairConfig>();
    private ScavCaseConfig ScavCase => configServer.GetConfig<ScavCaseConfig>();
    private SeasonalEventConfig SeasonalEvent => configServer.GetConfig<SeasonalEventConfig>();
    private TraderConfig Trader => configServer.GetConfig<TraderConfig>();
    private WeatherConfig Weather => configServer.GetConfig<WeatherConfig>();

    // insurance.json
    public void AddTraderReturnChance(MongoId Id, double Chance)
    {
        Insurance.ReturnChancePercent.TryAdd(Id, Chance);
    }
    
    // quest.json
    public void AddRepeatableQuestTraderWhitelist(MongoId Id, string NickName)
    {
        TraderWhitelist traderWhitelist = new()
        {
            TraderId = Id,
            Name = NickName,
            QuestTypes = [
                "Completion",
                "Exploration",
                "Elimination"
            ],
            RewardBaseWhitelist = [
                "543be6564bdc2df4348b4568",
                "5485a8684bdc2da71d8b4567",
                "590c745b86f7743cc433c5f2",
                "57864c322459775490116fbf",
                "57864a66245977548f04a81f",
                "5448f39d4bdc2d0a728b4568",
                "5448f3ac4bdc2dce718b4569",
                "5448f3a64bdc2d60728b456a",
                "57864c8c245977548867e7f1",
                "5448e8d04bdc2ddf718b4569",
                "57864e4c24597754843f8723",
                "57864ee62459775490116fc1",
                "543be5664bdc2dd4348b4569",
                "5a341c4086f77401f2541505",
                "5448e8d64bdc2dce718b4568",
                "55818ad54bdc2ddc698b4569",
                "57864a3d24597754843f8721",
                "5a341c4686f77469e155819e",
                "55818b224bdc2dde698b456f",
                "5c99f98d86f7745c314214b3",
                "55818aeb4bdc2ddc698b456a",
                "55818acf4bdc2dde698b456b",
                "57864bb7245977548b3b66c2",
                "5c164d2286f774194c5e69fa",
                "5448e5284bdc2dcb718b4567",
                "55818af64bdc2d5b648b4570",
                "5448bc234bdc2d3c308b4569",
                "55818b164bdc2ddc698b456c",
                "55818a684bdc2ddd698b456d",
                "550aa4cd4bdc2dd8348b456c",
                "550aa4dd4bdc2dc9348b4569",
                "55818a594bdc2db9688b456a",
                "55818a104bdc2db9688b4569",
                "5448e5724bdc2ddf718b4568",
                "5448e54d4bdc2dcc718b4568",
                "57bef4c42459772e8d35a53b",
                "5448e53e4bdc2d60728b4567",
                "5795f317245977243854e041",
                "5d650c3e815116009f6201d2",
                "57864ada245977548638de91",
            ],
            RewardCanBeWeapon = true,
            WeaponRewardChancePercent = 30
        };
        Quest.RepeatableQuests[0].TraderWhitelist.Add(traderWhitelist);
        Quest.RepeatableQuests[1].TraderWhitelist.Add(traderWhitelist);
    }
    
    // ragfair.json
    public void ApplyBaseFleaPrices()
    {
        Ragfair.Dynamic.GenerateBaseFleaPrices.UseHandbookPrice = false;
        Ragfair.Dynamic.GenerateBaseFleaPrices.PriceMultiplier = 1;
        Ragfair.Dynamic.GenerateBaseFleaPrices.PreventPriceBeingBelowTraderBuyPrice = false;
    }
    public void AddTraderRagfair(MongoId Id, bool flag = true)
    {
        Ragfair.Traders.TryAdd(Id, flag);
    }
    
    // trader.json
    public void SetTradersUpdateTime(int min, int? max=null, string? traderId=null, string? traderName=null)
    {
        var seconds = new MinMax<int>
        {
            Max = max??min,
            Min = min
        };
        int flag = 0;
        foreach (var key in Trader.UpdateTime)
        {
            if (string.IsNullOrEmpty(traderId) || key.TraderId == traderId)
            {
                key.Seconds = seconds;
                flag = 1;
            }
        }

        if (flag == 1) return;
        if (!MongoId.IsValidMongoId(traderId) || traderId == null) return;
        
        Trader.UpdateTime.Add(new UpdateTime()
        {
            Name =  traderName,
            TraderId = traderId,
            Seconds = seconds,
        });
    }

    // weather.json
    public void SetWeatherConfig(MGModConfig_Config_WeatherSettings value, string type = "default")
    {
        if (!Weather.Weather.PresetWeights.Keys.Contains(type)) return;
        var weather = Weather.Weather.PresetWeights[type];
        SetWeatherPresetWeightsType1(weather.Clouds, value.clouds);
        if(weather.WindSpeed != null) SetWeatherPresetWeightsType1(weather.WindSpeed, value.windSpeed);
        if(weather.Rain != null) SetWeatherPresetWeightsType1(weather.Rain, value.rain);
        if(weather.Fog != null) SetWeatherPresetWeightsType1(weather.Fog, value.fog);
    }

    public void SetWeatherPresetWeightsType1(Dictionary<string, double> presetWeights, MGModConfig_Config_Weather weatherType)
    {
        presetWeights.Clear();
		for (int ind = 0; ind < weatherType.values.Count; ind++)
		{
			presetWeights[weatherType.values[ind].ToString()] = weatherType.weights[ind];
		}
	}

    public void MGmodConfigs(MGModConfig_Config ConfigSetting)
    {
        // airdrop.json
        // 功能：空投种类 AirdropType
        if (ConfigSetting.AirdropType != "default")
        {
            var Type = ConfigSetting.AirdropType;
            var Weight = Airdrop.AirdropTypeWeightings;
            if(Type == "moreWeapon")
            {
                Weight[SptAirdropTypeEnum.weaponArmor] = 12;
            } 
            else if (Type == "moreFoodMedical")
            {
                Weight[SptAirdropTypeEnum.foodMedical] = 12;
            }
            else if (Type == "moreBarter")
            {
                Weight[SptAirdropTypeEnum.barter] = 12;
            }
            else if (Type == "moreMixed")
            {
                Weight[SptAirdropTypeEnum.mixed] = 9;
            }
        }

        // backup.json

        // bot.json
        // 功能：AI刷新数量 AISpawnNumber
        if (ConfigSetting.AISpawnNumber != 0)
        {
            foreach(var key in Bot.MaxBotCap.Keys)
            {
                Bot.MaxBotCap[key] += ConfigSetting.AISpawnNumber;
            }
        }

        // core.json
        // gifts.json
        // health.json
        // hideout.json
        // http.json

        // inraid.json
        // let inraid = MGConfigs.getConfig(ConfigTypes.IN_RAID);
        // 功能：战局默认选项 RaidDefault
        if (ConfigSetting.RaidDefault.enable)
        {
            InRaid.RaidMenuSettings.AiAmount = ConfigSetting.RaidDefault.aiAmount;
            InRaid.RaidMenuSettings.AiDifficulty = ConfigSetting.RaidDefault.aiDifficulty;
            InRaid.RaidMenuSettings.BossEnabled = ConfigSetting.RaidDefault.bossEnabled;
            InRaid.RaidMenuSettings.ScavWars = ConfigSetting.RaidDefault.scavWars;
            InRaid.RaidMenuSettings.TaggedAndCursed = ConfigSetting.RaidDefault.taggedAndCursed;
        }

        // insurance.json
        //功能：商人百分百回保 ReturnChance
        if(ConfigSetting.ReturnChance.enable)
        {
            var chance = ConfigSetting.ReturnChance.value;
            foreach(var key in Insurance.ReturnChancePercent.Keys)
            {
                Insurance.ReturnChancePercent[key] = chance;
            }
            Insurance.RunIntervalSeconds = 0;
        }

        // inventory.json
        // 功能：购买物品带钩 BuyFoundInRaid
        /*
        if (ConfigSetting.BuyFoundInRaid)
        {
            Inventory.NewItemsMarkedFound = ConfigSetting.BuyFoundInRaid;
        }
        */

        // item.json
        // locale.json

        // location.json
        // 功能：容器物资倍率 Container
        if (ConfigSetting.LootMultiple.Container != 1)
        {
            var multiplier = ConfigSetting.LootMultiple.Container;
            var staticsets = Location.StaticLootMultiplier;
            foreach(var key in staticsets.Keys)
            {
                staticsets[key] *= multiplier;
            }
        }
        // 功能：地面物资倍率 Ground
        if (ConfigSetting.LootMultiple.Ground != 1)
        {
            var multiplier = ConfigSetting.LootMultiple.Ground;
            var loosesets = Location.LooseLootMultiplier;
            foreach(var key in loosesets.Keys)
            {
                loosesets[key] *= multiplier;
            }
        }
        //功能：容器随机生成 RandomContainer
        if (ConfigSetting.RandomContainer)
        {
            Location.ContainerRandomisationSettings.Enabled = false; // 默认为开启随机， 所以若为true，则表示关闭随机
            foreach (var map in Location.ContainerRandomisationSettings.Maps.Keys)
            {
                Location.ContainerRandomisationSettings.Maps[map] = false;
            }
        }

        // loot.json
        
        // lostondeath.json
        // 功能： 死亡不掉落 NoLostonDeath
        if (ConfigSetting.NoLostonDeath)
        {
            LostOnDeath.Equipment.Headwear = false;
            LostOnDeath.Equipment.Earpiece = false;
            LostOnDeath.Equipment.FaceCover = false;
            LostOnDeath.Equipment.ArmorVest = false;
            LostOnDeath.Equipment.Eyewear = false;
            LostOnDeath.Equipment.TacticalVest = false;
            LostOnDeath.Equipment.PocketItems = false;
            LostOnDeath.Equipment.Backpack = false;
            LostOnDeath.Equipment.Holster = false;
            LostOnDeath.Equipment.FirstPrimaryWeapon = false;
            LostOnDeath.Equipment.SecondPrimaryWeapon = false;
            LostOnDeath.QuestItems = false;
        }
        
        // match.json
        
        // playerscav.json
        // 功能： Scav装备优化 ScavEquipmentOptimize
        if (ConfigSetting.ScavEquipmentOptimize)
        {
            foreach (var level in PlayerScav.KarmaLevel.Keys)
            {
                int addValue = 0;
                if (int.TryParse(level, out int x))
                {
                    addValue = x;
                }
                foreach (var equipment in PlayerScav.KarmaLevel[level].Modifiers.Equipment.Keys)
                {
                    PlayerScav.KarmaLevel[level].Modifiers.Equipment[equipment] += (addValue + 8) * 3;
                }
                foreach (var mod in PlayerScav.KarmaLevel[level].Modifiers.Mod.Keys)
                {
                    PlayerScav.KarmaLevel[level].Modifiers.Mod[mod] += (addValue + 8) * 3;
                }
            }
        }
        // pmc.json
        // 功能：USEC比例 USECRate
        if (ConfigSetting.USECRate.enable)
        {
            Pmc.IsUsec = ConfigSetting.USECRate.value;
        }

        // pmcchatresponse.json
        // quest.json

        // ragfair.json
        // 功能：跳蚤出售100% Sell100
        if (ConfigSetting.Sell100)
        {
            var RagfairSellChance = Ragfair.Sell.Chance;
            RagfairSellChance.Base = 100;
            RagfairSellChance.SellMultiplier = 2;
            RagfairSellChance.MaxSellChancePercent = 100;
            RagfairSellChance.MinSellChancePercent = 100;
        }
        // 功能：跳蚤极速出售 SellFast
        if (ConfigSetting.SellFast)
        {
            Ragfair.Sell.Time = new MinMax<double>
            {
                Max = 0.01,
                Min = 0
            };
        }
        // 功能：购买物品带钩 BuyFoundInRaid
        if (ConfigSetting.BuyFoundInRaid)
        {
            Ragfair.Dynamic.PurchasesAreFoundInRaid = ConfigSetting.BuyFoundInRaid;
        }
        // 功能：跳蚤购买优化 SellOptimize
        if (ConfigSetting.SellOptimize)
        {
            var RagfairDynamic = Ragfair.Dynamic;
            // 跳蚤不可堆叠物品出售数量
            RagfairDynamic.NonStackableCount = new MinMax<int>
            {
                Max = 5000,
                Min = 100
            };
            // 跳蚤可堆叠物品出售数量
            RagfairDynamic.StackablePercent = new MinMax<double>
            {
                Max = 50000,
                Min = 500
            };
            // // 跳蚤显示为单个物品的
            RagfairDynamic.ShowAsSingleStack = new HashSet<MongoId> { };
            // 护甲没有插板概率
            RagfairDynamic.Armor.RemoveRemovablePlateChance = 0;
        }
        // 功能：跳蚤物品全新 SellNew
        if (ConfigSetting.SellNew)
        {
            var RagfairDynamicCondition = Ragfair.Dynamic.Condition;
            foreach (var key in RagfairDynamicCondition.Keys)
            {
                RagfairDynamicCondition[key].ConditionChance = 0;
            }
        }
        // 功能：禁用跳蚤黑名单 NoBlackList
        if (ConfigSetting.NoBlackList)
        {
            Ragfair.Dynamic.Blacklist.EnableBsgList = !ConfigSetting.NoBlackList;
        }

        // repair.json
        // 功能：护甲附魔
        if (ConfigSetting.Buffs.BuffsArmor)
        {
            var RepairKit = Repair.RepairKit;
            var RarityWeight = new Dictionary<string, double>
            {
                { "Common", 0},
                { "Rare", 100}
            };
            RepairKit.Armor.RarityWeight = RarityWeight;
            RepairKit.Vest.RarityWeight = RarityWeight;
            RepairKit.Headwear.RarityWeight = RarityWeight;
            Repair.ArmorKitSkillPointGainPerRepairPointMultiplier *= 100;
        }
        // 功能：武器附魔
        if (ConfigSetting.Buffs.BuffsWeapon)
        {
            var RepairKit = Repair.RepairKit;
            var RarityWeight = new Dictionary<string, double>
            {
                { "Common", 0},
                { "Rare", 100}
            };
            RepairKit.Weapon.RarityWeight = RarityWeight;
            //Repair.WeaponSkillRepairGain *= 100;
        }
        // 功能：附魔
        if (ConfigSetting.Buffs.BuffsWeapon || ConfigSetting.Buffs.BuffsArmor)
        {
            Repair.RepairKitIntellectGainMultiplier.Weapon = 100;
            Repair.RepairKitIntellectGainMultiplier.Armor = 100;
            Repair.MaxIntellectGainPerRepair.Kit = 1;
            Repair.MaxIntellectGainPerRepair.Trader = 1;
        }

        // scavcase.json
        // seasonalevents.json

        // trader.json
        // 功能：商人供货时间 UpdateTime
        if (ConfigSetting.UpdateTime.enable)
        {
            var updateTime = ConfigSetting.UpdateTime.value;
            Trader.UpdateTimeDefault = updateTime;
            SetTradersUpdateTime(updateTime);
        }
        // 功能：购买物品带钩 BuyFoundInRaid
        if (ConfigSetting.BuyFoundInRaid)
        {
            Trader.PurchasesAreFoundInRaid = ConfigSetting.BuyFoundInRaid;
        }

        // weather.json
        // 功能：天气修改
        if (ConfigSetting.WeatherSettings.mode != "default")
        {
            HashSet<String> weatherType = ["SUNNY","RAINY","CLOUDY","WINTER"];
            foreach (var weather in weatherType)
            {
                SetWeatherConfig(ConfigSetting.WeatherSettings, weather);
            }
        }
    }
}
