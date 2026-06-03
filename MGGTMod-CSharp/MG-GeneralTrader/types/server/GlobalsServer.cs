using _MGMod.types.models.Custom;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace _MGMod.types.server;

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
    public void MGmodGlobals(MGModConfig_Globals GlobalsSetting)
    {
        // 功能：撤离时间无限制 EscapeNoTimeLimit
        if (GlobalsSetting.EscapeNoTimeLimit)
        {
            Globals.Configuration.Exp.MatchEnd.SurvivedExperienceRequirement = 0;
        }
        // 功能：跳蚤开放等级 FleaMarketOpenLevel
        if (GlobalsSetting.FleaMarketOpenLevel)
        {
            Globals.Configuration.RagFair.MinUserLevel = 1;
        }
        // 功能：去除战局携带限制 TakeLimit
        if (GlobalsSetting.TakeLimit)
        {
            Globals.Configuration.RestrictionsInRaid = [];
        }
        // 功能：Scav无冷却 ScavOptimize
        if(GlobalsSetting.ScavOptimize)
        {
            //SCAV冷却时间
            Globals.Configuration.SavagePlayCooldown = 0;
            //SCAV剩余冷却时间
            Globals.Configuration.SavagePlayCooldownNdaFree = 0;
            //减少SCAV冷却时间
            Globals.Configuration.SavagePlayCooldownDevelop = 0;
        }
        // 功能：极低税率 LowTaxRate
        if (GlobalsSetting.LowTaxRate)
        {
            Globals.Configuration.RagFair.CommunityTax = (float)0.01;
            Globals.Configuration.RagFair.CommunityItemTax = (float)0.01;
            Globals.Configuration.RagFair.CommunityRequirementTax = (float)0.01;
        }
        // 功能：跳蚤交易单倍率 SellNumber
        if (GlobalsSetting.SellNumber != 1)
        {
            foreach(var item in Globals.Configuration.RagFair.MaxActiveOfferCount)
            {
                item.Count *= GlobalsSetting.SellNumber;
            }
        }
        // 功能：装卸速度 LoadSpeed
        if (GlobalsSetting.LoadSpeed.mode != "default")
        {
            // 压弹速度
            Globals.Configuration.BaseLoadTime = GlobalsSetting.LoadSpeed.BaseLoadTime;
            // 卸弹速度
            Globals.Configuration.BaseUnloadTime = GlobalsSetting.LoadSpeed.BaseUnloadTime;
        }
        // 功能：超人模式 SuperHero
        if (GlobalsSetting.SuperHero)
        {
            var Stamina = Globals.Configuration.Stamina;
            Stamina.BaseRestorationRate = 50;
            Stamina.Capacity = 2800;
            Stamina.HandsCapacity = 2000;
            Stamina.OxygenCapacity = 2000;
            Stamina.OxygenRestoration = 200;
            Globals.Configuration.BaseLoadTime = 0.01;
            Globals.Configuration.BaseUnloadTime = 0.01;
        }
        // 功能：物资倍率 Value
        if (GlobalsSetting.LootMultiplier.Value != 1)
        {
            Globals.Configuration.GlobalItemPriceModifier = GlobalsSetting.LootMultiplier.Value;
        }
        // 全局物资倍率 Global
        if (GlobalsSetting.LootMultiplier.Global != 1)
        {
            Globals.Configuration.GlobalLootChanceModifier = GlobalsSetting.LootMultiplier.Global;
            Globals.Configuration.GlobalLootChanceModifierPvE = GlobalsSetting.LootMultiplier.Global;
        }
        // 功能：护甲维修无损耗 ArmorRepairPerfect
        if (GlobalsSetting.ArmorRepairPerfect)
        {
            foreach(var key in Globals.Configuration.ArmorMaterials.Keys)
            {
                Globals.Configuration.ArmorMaterials[key].MinRepairDegradation = 0;
                Globals.Configuration.ArmorMaterials[key].MaxRepairDegradation = 0;
                Globals.Configuration.ArmorMaterials[key].MinRepairKitDegradation = 0;
                Globals.Configuration.ArmorMaterials[key].MaxRepairKitDegradation = 0;
            }
        }
        // 功能：附魔
        //100%护甲附魔
        if (GlobalsSetting.Buffs.BuffsArmor)
        {
            var skillsettings = Globals.Configuration.SkillsSettings;
            double chance = 100;

            skillsettings.HeavyVests.BuffSettings.CommonBuffChanceLevelBonus = chance / 100;
            skillsettings.HeavyVests.BuffSettings.CommonBuffMinChanceValue = chance / 100;
            skillsettings.HeavyVests.BuffSettings.RareBuffChanceCoff = chance / 100;
            skillsettings.HeavyVests.BuffSettings.CurrentDurabilityLossToRemoveBuff = 1;
            skillsettings.HeavyVests.BuffSettings.MaxDurabilityLossToRemoveBuff = 1;
            skillsettings.HeavyVests.BuffSettings.ReceivedDurabilityMaxPercent = chance;

            skillsettings.LightVests.BuffSettings.CommonBuffChanceLevelBonus = chance / 100;
            skillsettings.LightVests.BuffSettings.CommonBuffMinChanceValue = chance / 100;
            skillsettings.LightVests.BuffSettings.RareBuffChanceCoff = chance / 100;
            skillsettings.LightVests.BuffSettings.CurrentDurabilityLossToRemoveBuff = 1;
            skillsettings.LightVests.BuffSettings.MaxDurabilityLossToRemoveBuff = 1;
            skillsettings.LightVests.BuffSettings.ReceivedDurabilityMaxPercent = chance;

            skillsettings.Melee.BuffSettings.CommonBuffChanceLevelBonus = chance / 100;
            skillsettings.Melee.BuffSettings.CommonBuffMinChanceValue = chance / 100;
            skillsettings.Melee.BuffSettings.RareBuffChanceCoff = chance / 100;
            skillsettings.Melee.BuffSettings.CurrentDurabilityLossToRemoveBuff = 1;
            skillsettings.Melee.BuffSettings.MaxDurabilityLossToRemoveBuff = 1;
            skillsettings.Melee.BuffSettings.ReceivedDurabilityMaxPercent = chance;

            Globals.Configuration.RepairSettings.MinimumLevelToApplyBuff = 0;
        }
        //100%枪械附魔
        if (GlobalsSetting.Buffs.BuffsWeapon)
        {
            var skillsettings = Globals.Configuration.SkillsSettings;
            double chance = 100;

            skillsettings.WeaponTreatment.BuffSettings.CommonBuffMinChanceValue = chance / 100;
            skillsettings.WeaponTreatment.BuffSettings.CommonBuffChanceLevelBonus = chance / 100;
            skillsettings.WeaponTreatment.BuffSettings.CurrentDurabilityLossToRemoveBuff = 1;
            skillsettings.WeaponTreatment.BuffSettings.MaxDurabilityLossToRemoveBuff = 1;
            skillsettings.WeaponTreatment.BuffSettings.RareBuffChanceCoff = chance / 100;
            skillsettings.WeaponTreatment.BuffSettings.ReceivedDurabilityMaxPercent = chance;
            skillsettings.WeaponTreatment.SkillPointsPerRepair = 5000;
            
            Globals.Configuration.RepairSettings.MinimumLevelToApplyBuff = 0;
        }
        // 功能：练技能速度 ExpOptimize
        if (GlobalsSetting.ExpOptimize)
        {
            Globals.Configuration.SkillEnduranceWeightThreshold = 0.65;   // 耐力技能增长条件：0.1*最大负重时
            Globals.Configuration.SkillFatiguePerPoint = 1;    // 疲劳因子  >=1 则没有疲劳
            Globals.Configuration.SkillFatigueReset = 0;   // 疲劳结束冷却时间 (s)
            Globals.Configuration.SkillFreshEffectiveness = 1.5;   // 疲劳冷却后 技能升级加速300%
            Globals.Configuration.SkillMinEffectiveness = 0.1;   //最低获得技能点数
            Globals.Configuration.SkillPointsBeforeFatigue = 5; //技能疲劳前可以升多少级
            Globals.Configuration.SkillsSettings.SkillProgressRate = 2; //全局技能速率
            Globals.Configuration.SkillsSettings.WeaponSkillProgressRate = 5; //全局武器技能速率
            Globals.Configuration.SkillsSettings.WeaponSkillRecoilBonusPerLevel = 0.1; //每级的全局武器技能速率加成
        }
    }
}
