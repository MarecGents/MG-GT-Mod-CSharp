using _MGMod.types.models.Custom;
using _MGMod.types.models.EFT.locations;
using _MGMod.types.utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Server;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace _MGMod.types.server;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class LocationsServer
{
    private DatabaseService databaseService;
    private ISptLogger<LocationsServer> logger;
    private MGUtils mGUtils;
    private Locations Locations => databaseService.GetLocations();
    
    public LocationsServer(
        DatabaseService _databaseService,
        ISptLogger<LocationsServer> _logger,
        MGUtils _mGUtils
    )
    {
        databaseService =  _databaseService;
        logger =  _logger;
        mGUtils =  _mGUtils;
    }
    
    public Dictionary<string, Location> GetLocations()
    {
        return Locations.GetDictionary();
    }
    
    public void MGmodLocations(MGModConfig_Locations LocationsSetting)
    {
        var Locations_ = Locations.GetDictionary();
        string[] Exclude = [ "Develop", "Hideout", "PrivateArea", "Suburbs", "Terminal", "Town"];
        foreach (var mapName  in Locations_.Keys)
        {
            if (Exclude.Contains(mapName)) continue;
            // 功能：战局时长(分钟) RaidTime
            if (LocationsSetting.RaidTime.enable)
            {
                Locations_[mapName].Base.EscapeTimeLimit = LocationsSetting.RaidTime.value;
            }
            // 功能：BOSS刷新率 BOSSSpwanChance
            if (LocationsSetting.BOSSSpwanChance.enable && Locations_[mapName].Base.BossLocationSpawn != null)
            {
                foreach(var Bzone in Locations_[mapName].Base.BossLocationSpawn)
                {
                    if (Bzone.BossName.IndexOf("boss") == 0 || Bzone.Supports != null)
                    {
                        Bzone.BossChance = LocationsSetting.BOSSSpwanChance.value;
                    }
                }
            }
            // 功能：100%可拉闸  功能：100%可撤离
            if ((LocationsSetting.Pass100 || LocationsSetting.Escape100) && Locations_[mapName].Base.Exits != null)
            {
                foreach(var exit in Locations_[mapName].Base.Exits)
                {
                    if (exit.PassageRequirement == null) continue;
                    if (exit.PassageRequirement == RequirementState.WorldEvent && LocationsSetting.Pass100)
                    {
                        exit.Chance = 100;
                    }
                    else if (LocationsSetting.Escape100)
                    {
                        exit.Chance = 100;
                    }

                }
            }
            // 功能：地图是否回保 MapInsurance
            if (LocationsSetting.MapInsurance.ContainsKey(mapName))
            {
                Locations_[mapName].Base.Insurance = LocationsSetting.MapInsurance[mapName];
                Locations_[mapName].Base.IsSecret = !LocationsSetting.MapInsurance[mapName];
            }
            
        }
    }
}
