using _MGGTmod.types.models.Custom;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace _MGGTmod.types.server;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class GlobalsServer(
    DatabaseService databaseService,
    ISptLogger<GlobalsServer> logger
    )
{
    private Globals Globals => databaseService.GetGlobals();

    public Globals GetGlobals()
    {
        return Globals;
    }
    public void AddBuff(string buffName, List<Buff> buff)
    {
        var Buffs = Globals.Configuration.Health.Effects.Stimulator.Buffs;
        if (!Buffs.ContainsKey(buffName))
        {
            Buffs.Add(buffName, buff);
            return;
        }
        logger.LogWithColor($"针剂Buff名称：{buffName}重复！请更换其他Buff名称。", LogTextColor.Cyan);
        return;

    }
    public void AddBuffs(Dictionary<string,List<Buff>> Buffs)
    {
        foreach( var buff in Buffs)
        {
            AddBuff(buff.Key, buff.Value);
        }
    }
    
}
