using _MGGTmod.types.models.Custom;
using _MGGTmod.types.models.Paths;
using _MGGTmod.types.server;
using _MGGTmod.types.utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services.Mod;

namespace _MGGTmod.types.services;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class ConfigSettingServices
{
    private ISptLogger<ConfigSettingServices> logger;
    private MGUtils mGUtils;
    private CustomTraderServices  customTraderServices;
    private TestServices testServices;
    public ConfigSettingServices(
        ISptLogger<ConfigSettingServices> _logger,
        MGUtils _mGUtils,
        CustomTraderServices _customTraderServices,
        TestServices _testServices
        )
    {
        logger = _logger;
        mGUtils = _mGUtils;
        customTraderServices = _customTraderServices;
        testServices = _testServices;
    }

    public async Task ModSetting()
    {
        customTraderServices.Start();
        mGUtils.Log_GT("商人系统", "加载完毕。", LogTextColor.Green);
    }
}
