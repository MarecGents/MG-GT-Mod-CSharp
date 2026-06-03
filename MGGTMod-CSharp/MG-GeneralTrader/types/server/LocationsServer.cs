using _MGGTmod.types.models.Custom;
using _MGGTmod.types.models.EFT.locations;
using _MGGTmod.types.utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Server;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace _MGGTmod.types.server;

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
    
}
