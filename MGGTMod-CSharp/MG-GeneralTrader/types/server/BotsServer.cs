using _MGMod.types.models.Custom;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Spt.Bots;
using SPTarkov.Server.Core.Services;

namespace _MGMod.types.server;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class BotsServer(
    DatabaseService databaseService
    )
{
    private Bots Bots => databaseService.GetBots();

    public Bots GetBots()
    {
        var bots = Bots;
        return bots;
    }

    public void SetBotsHealth(int rate, string? botType = null)
    {
        foreach(var key in Bots.Types.Keys)
        {
            if( string.IsNullOrEmpty(botType) || key == botType)
            {
                var bodyPart = Bots.Types[key].BotHealth.BodyParts.ElementAtOrDefault(0);
				bodyPart.Chest.Max *= rate;
                bodyPart.Chest.Min *= rate;
                bodyPart.Head.Max *= rate;
                bodyPart.Head.Min *= rate;
                bodyPart.LeftLeg.Max *= rate;
                bodyPart.LeftLeg.Min *= rate;
                bodyPart.LeftArm.Max *= rate;
                bodyPart.LeftArm.Min *= rate;
                bodyPart.RightLeg.Max *= rate;
                bodyPart.RightLeg.Min *= rate;
                bodyPart.RightArm.Max *= rate;
                bodyPart.RightArm.Min *= rate;
                bodyPart.Stomach.Max *= rate;
                bodyPart.Stomach.Min *= rate;
            }
        }
    }
    public void MGmodBots(MGModConfig_Bot? BotSetting)
    {
        // 功能：AI血量 AIHealth
        if ( BotSetting?.AIHealth != 1)
        {
            SetBotsHealth(BotSetting.AIHealth);
        }
    }
}
