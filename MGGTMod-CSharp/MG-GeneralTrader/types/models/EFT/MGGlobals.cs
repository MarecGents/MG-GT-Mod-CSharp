using SPTarkov.Server.Core.Models.Eft.Common;

namespace _MGGTmod.types.models.EFT;
public interface ICustomItemBuffs
{
    public Dictionary<string, List<Buff>>? Buffs { get; set; }
}

public class CustomGlobals: ICustomItemBuffs
{
    public Dictionary<string, Preset>?  ItemPresets { get; set; }
    public Dictionary<string, List<Buff>>? Buffs { get; set; }
}
