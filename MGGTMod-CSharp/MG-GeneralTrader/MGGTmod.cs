using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Helpers;
using _MGGTmod.types.services;
using _MGGTmod.types.utils;

namespace _MGGTmod;

public record ModMetadata : AbstractModMetadata
{
	public override string ModGuid { get; init; } = "com.marecgents.tarkovmod.mggtmod";
	public override string Name { get; init; } = "MGGTMod";
	public override string Author { get; init; } = "MarecGents";
	public override List<string>? Contributors { get; init; }
	public override SemanticVersioning.Version Version { get; init; } = new("0.3.0");
	public override SemanticVersioning.Range SptVersion { get; init; } = new("4.0.13");
	public override List<string>? Incompatibilities { get; init; }
	public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; } = "https://github.com/MarecGents/MG-GT-Mod-CSharp/";
    public override bool? IsBundleMod { get; init; } = true;
	public override string? License { get; init; } = "MIT";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class MGGTmod(
    ISptLogger<MGGTmod> logger,
    ModHelper modHelper,
    ConfigSettingServices configSettingServices,
    MGUtils  mGUtils
    ) : IOnLoad
{
    public async Task OnLoad()
    {
        // var modPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        // logger.LogWithColor("This is MGmod", LogTextColor.Red);
        await configSettingServices.ModSetting();
    }
}

[Injectable(TypePriority = OnLoadOrder.PreSptModLoader + 1)]
public class PreMGmodLoad(
    ISptLogger<PreMGmodLoad> logger
    ) : IOnLoad
{
    public Task OnLoad()
    {
        // logger.LogWithColor("This is PreMGmodLoad", LogTextColor.Red, LogBackgroundColor.Cyan);
        return Task.CompletedTask;
    }
}