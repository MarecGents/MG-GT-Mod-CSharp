using _MGMod.types.utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using _MGMod.types.server;

namespace _MGMod.types.services;
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]

public class CustomProfileServices
{
	private ISptLogger<CustomProfileServices> logger;
	private DatabaseService databaseService;
	private TemplatesServer templatesServer;
	private MGUtils mGUtils;

	public CustomProfileServices(
		ISptLogger<CustomProfileServices> _logger,
		DatabaseService _databaseService,
		TemplatesServer _templatesServer,
		MGUtils _mGUtils
		)
	{
		logger = _logger;
		databaseService = _databaseService;
		templatesServer = _templatesServer;
		mGUtils = _mGUtils;
	}

	public void start()
	{
		
	}
}
