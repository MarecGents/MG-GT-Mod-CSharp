using _MGGTmod.types.models.Custom;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;

namespace _MGGTmod.types.server;

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
    
}
