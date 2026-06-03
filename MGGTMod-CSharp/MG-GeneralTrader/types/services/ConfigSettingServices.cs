using _MGMod.types.models.Custom;
using _MGMod.types.models.Paths;
using _MGMod.types.server;
using _MGMod.types.utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services.Mod;

namespace _MGMod.types.services;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class ConfigSettingServices
{
    private ISptLogger<ConfigSettingServices> logger;
    private ConfigSettingType? configJson;
    private MGUtils mGUtils;

    private BotsServer botsServer;
    private ConfigsServer configsServer;
    private GlobalsServer globalsServer;
    private HideoutServer hideoutServer;
    private LocationsServer locationsServer;
    private TemplatesServer templatesServer;
    private TradersServer tradersServer;

    private CustomItemServices customItemServices;
    private KeyClassfyServices keyClassfyServices;
    private SyncFleaMarketServices syncFleaMarketServices;
    private CustomTraderServices  customTraderServices;
    private TestServices testServices;
    public ConfigSettingServices(
        ISptLogger<ConfigSettingServices> _logger,
        MGUtils _mGUtils,

        BotsServer _botsServer,
        ConfigsServer _configsServer,
        GlobalsServer _globalsServer,
        HideoutServer _hideoutServer,
        LocationsServer _locationsServer,
        TemplatesServer _templatesServer,
        TradersServer _tradersServer,
        
        CustomTraderServices _customTraderServices,
        CustomItemServices _customItemServices,
        KeyClassfyServices _keyClassfyServices,
        SyncFleaMarketServices _syncFleaMarketServices,
            
        TestServices _testServices
        )
    {
        logger = _logger;
        mGUtils = _mGUtils;

        botsServer = _botsServer;
        configsServer = _configsServer;
        globalsServer = _globalsServer;
        hideoutServer = _hideoutServer;
        locationsServer = _locationsServer;
        templatesServer = _templatesServer;
        tradersServer = _tradersServer;

        customTraderServices = _customTraderServices;
        customItemServices = _customItemServices;
        keyClassfyServices = _keyClassfyServices;
        syncFleaMarketServices = _syncFleaMarketServices;
        
        testServices = _testServices;
        
        configJson = mGUtils.GetJsonDataFromFile<ConfigSettingType>(Paths.ConfigJson);
    }

    public async Task ModSetting()
    {
        var CustomSetting = GetMGCustomSetting();
        // testServices.Initialize();
        if (CustomSetting.SyncFlea) await syncFleaMarketServices.Start();
        if (CustomSetting.CustomTrader) customTraderServices.Start();
        if (CustomSetting.CustomItem) customItemServices.Start();
        if (CustomSetting.KeyClassfy) keyClassfyServices.Start();
         botsServer.MGmodBots(GetBotSetting());
         configsServer.MGmodConfigs(GetConfigSetting());
         globalsServer.MGmodGlobals(GetGlobalsSetting());
         hideoutServer.MGmodHideout(GetHideoutSetting());
         locationsServer.MGmodLocations(GetLocationsSetting());
         templatesServer.MGmodTemplates(GetTemplatesSetting());
         tradersServer.MGmodTraders(GetTradersSetting());
         mGUtils.Log("常规设置", "已开启。", LogTextColor.Yellow);
    }

    private MGModConfig_Bot? GetBotSetting()
    { 
        if(configJson != null)
        {
            return configJson.Bot;
        }
        return null;
    }

    private MGModConfig_Config? GetConfigSetting()
    {
        if (configJson != null)
        {
            return configJson.Config;
        }
        return null;
    }
    private MGModConfig_Globals? GetGlobalsSetting()
    {
        if (configJson != null)
        {
            return configJson.Globals;
        }
        return null;

    }
    private MGModConfig_Hideout? GetHideoutSetting()
    {
        if (configJson != null)
        {
            return configJson.Hideout;
        }
        return null;
    }
    private MGModConfig_Locations? GetLocationsSetting()
    {
        if (configJson != null)
        {
            return configJson.Locations;
        }
        return null;
    }
    private MGModConfig_Templates? GetTemplatesSetting()
    {
        if (configJson != null)
        {
            return configJson.Templates;
        }
        return null;
    }
    private MGModConfig_Traders? GetTradersSetting()
    {
        if (configJson != null)
        {
            return configJson.Traders;
        }
        return null;
    }
    private MGModConfig_MGCustom? GetMGCustomSetting()
    {
        if (configJson != null)
        {
            return configJson.MGCustom;
        }
        return null;
    }
}
