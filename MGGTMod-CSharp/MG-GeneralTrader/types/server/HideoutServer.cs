using _MGMod.types.models.Custom;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Hideout;
using SPTarkov.Server.Core.Models.Enums.Hideout;
using SPTarkov.Server.Core.Models.Spt.Hideout;
using SPTarkov.Server.Core.Services;

namespace _MGMod.types.server;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class HideoutServer(
    DatabaseService databaseService
    )
{
    private Hideout Hideout => databaseService.GetHideout();

    public Hideout GetHideout()
    {
        return Hideout;
    }

    private void SetConstructionTime(double value)
    {
        foreach(var area in Hideout.Areas)
        {
            foreach(var n in area.Stages.Keys)
            {
                if(area.Stages[n].ConstructionTime != 0)
                {
                    area.Stages[n].ConstructionTime = value;
                }
            }
        }
    }
    private void SetProductionTime(double value)
    {
        foreach(var product in Hideout.Production.Recipes)
        {
            if(product.ProductionTime != 0)
            {
                product.ProductionTime = value;
            }
        }
    }
    private void SetScavecaseTime(double value)
    {
        foreach(var scavcase in Hideout.Production.ScavRecipes)
        {
            if(scavcase.ProductionTime != 0)
            {
                scavcase.ProductionTime = value;
            }
        }
    }
    private void SetUpgradeNoLimit()
    {
        foreach(var area in Hideout.Areas)
        {
            foreach(var n in area.Stages.Keys)
            {
                area.Stages[n].Requirements = [];
            }
        }
    }
    private void SetBonusesLevel(int value)
    {
        List<String> RealValue = [
            "EnergyRegeneration", 
            "HydrationRegeneration",
            "HealthRegeneration",
            "MaximumEnergyReserve",
            "StashSize",
        ];
        List<String> AddPercent = [
            "DebuffEndDelay",
            "ExperienceRate",
            "SkillGroupLevelingBoost",
            "QuestMoneyReward",
            "RepairWeaponBonus",
            "RepairArmorBonus",
        ];
        List<String> ReducePercent = [
            "FuelConsumption",
            "ScavCooldownTimer",
            "InsuranceReturnTime",
            "RagfairCommission",
        ];
        List<String> PassItem = [
            "AdditionalSlots",
            "UnlockWeaponModification",
            "UnlockWeaponRepair",
            "UnlockArmorRepair",
            "TextBonus",
        ];

        List<double> TimesValue = [1, 2, 5, 10];
        List<double> AddorReducePercent = [0, 10, 20, 50];
        
        foreach(var area in Hideout.Areas)
        {
            foreach(var n in area.Stages.Keys)
            {
                foreach (var bonus in area.Stages[n].Bonuses)
                {
                    if (RealValue.Contains(bonus.Type.ToString()))
                    {
                        bonus.Value *= TimesValue[value-1];
                    }
                    else if (AddPercent.Contains(bonus.Type.ToString()))
                    {
                        bonus.Value += AddorReducePercent[value-1];
                    }
                    else if (ReducePercent.Contains(bonus.Type.ToString()))
                    {
                        bonus.Value -= AddorReducePercent[value-1];
                    }
                    else if (PassItem.Contains(bonus.Type.ToString()))
                    {
                        continue;
                    }
                }
            }
        }
    }
    private void SetNoNeedsFuel()
    {
        foreach(var area in Hideout.Areas)
        {
            area.NeedsFuel = false;
        }
    }
    private void SetQteSucess100()
    {
        foreach (var qte in Hideout.Qte)
        {
            foreach (var quickTimeEvents in qte.QuickTimeEvents)
            {
                quickTimeEvents.Coordinates.X = 0.5f;
                quickTimeEvents.Coordinates.Y = 0.25f;
                quickTimeEvents.SuccessCoordinates.X = 0.5f;
                quickTimeEvents.SuccessCoordinates.Y = 0.25f;
                quickTimeEvents.MovementSpeed = 1;
            }
        }
    }
    private void SetQteNoPunish()
    {
        foreach (var qte in Hideout.Qte)
        {
            foreach (var n in qte.Results.Keys)
            {
                qte.Results[n].Energy = 0;
                qte.Results[n].Hydration = 0;
                qte.Results[QteEffectType.finishEffect].RewardEffects[0].Time = 1;
                qte.Results[QteEffectType.singleFailEffect].RewardEffects = [];
            }
        }
    }
    private void SetQteRewardMultiple(int value)
    {
        foreach (var qte in Hideout.Qte)
        {
            foreach (var rewardEffects in qte.Results[QteEffectType.singleSuccessEffect].RewardEffects)
            {
                foreach (var levelMultiplier in rewardEffects.LevelMultipliers)
                {
                    levelMultiplier.MultiplierValue *= value;
                }
            }
        }
    }
    public void MGmodHideout(MGModConfig_Hideout HideoutSetting)
    {
        // 功能：藏身处升级时间 BuildTime
        if (HideoutSetting.BuildTime.enable)
        {
            SetConstructionTime(HideoutSetting.BuildTime.value);
        }
        // 功能：藏身处生产时间 ProductTime
        if (HideoutSetting.ProductTime.enable)
        {
            SetProductionTime(HideoutSetting.ProductTime.value);
        }
        // 功能：Scav宝箱
        if (HideoutSetting.ScavCaseTime.enable)
        {
            SetScavecaseTime(HideoutSetting.ScavCaseTime.value);
        }
        // 功能：藏身处升级无限制 UpgradeNoLimit
        if (HideoutSetting.UpgradeNoLimit)
        {
            SetUpgradeNoLimit();
        }
        // 功能：藏身处区域加成等级 BonusesLevel
        if (HideoutSetting.BonusesLevel.enable)
        {
            SetBonusesLevel(HideoutSetting.BonusesLevel.value);
        }
        // 功能：藏身处区域无需供电 NoNeedsFuel
        if (HideoutSetting.NoNeedsFuel)
        {
            SetNoNeedsFuel();
        }
        // 功能：健身房锻炼百分百成功 Sucess100
        if (HideoutSetting.Qte.Sucess100)
        {
            SetQteSucess100();
        }
        // 功能：健身房锻炼无惩罚 NoPunish
        if (HideoutSetting.Qte.NoPunish)
        {
            SetQteNoPunish();
        }
        // 功能：健身房锻炼奖励倍率 RewardMultiple
        if (HideoutSetting.Qte.RewardMultiple.enable)
        {
            SetQteRewardMultiple(HideoutSetting.Qte.RewardMultiple.value);
        }
    }
}
