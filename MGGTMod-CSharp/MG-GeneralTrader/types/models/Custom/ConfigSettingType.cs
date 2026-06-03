using SPTarkov.Server.Core.Models.Common;

namespace _MGMod.types.models.Custom;

public class ConfigSettingType
{
    public MGModConfig_Bot Bot { get; set; }
    public MGModConfig_Config Config { get; set; }
    public MGModConfig_Globals Globals { get; set; }
    public MGModConfig_Hideout Hideout { get; set; }
    public MGModConfig_Locations Locations { get; set; }
    public MGModConfig_Templates Templates { get; set; }
    public MGModConfig_Traders Traders { get; set; }
    public MGModConfig_MGCustom MGCustom { get; set; }
    public string saveTime { get; set; }

}

public class MGModConfig_Bot
{
    public int AIHealth { get; set; }
}

public class MGModConfig_Config
{
    public string AirdropType { get; set; }
    public int AISpawnNumber { get; set; }
    public MGModConfig_Config_RaidDefault RaidDefault {  get; set; }
    public EnableValueInt ReturnChance {  get; set; }  // string | double
    public bool BuyFoundInRaid { get; set; }
    public  MGModConfig_Config_LootMultiple LootMultiple {  get; set; }
    public bool RandomContainer { get; set; }
    public EnableValueInt USECRate {  get; set; }
    public bool Sell100 {  get; set; }
    public bool SellFast {  get; set; }
    public bool SellOptimize {  get; set; }
    public bool SellNew {  get; set; }
    public bool NoBlackList {  get; set; }
    public MGModConfig_Config_Buffs Buffs {  get; set; }
    public EnableValueInt UpdateTime {  get; set; }    // string | int
    public MGModConfig_Config_WeatherSettings WeatherSettings {  get; set; }
    public bool NoLostonDeath {  get; set; }
    public bool ScavEquipmentOptimize { get; set; }
}

public class MGModConfig_Config_RaidDefault
{
    public bool enable {  get; set; }
    public string aiAmount { get; set; }
    public string aiDifficulty { get; set; }
    public bool bossEnabled { get; set; }
    public bool scavWars { get; set; }
    public bool taggedAndCursed { get; set; }
    public bool enablePve { get; set; }
    public bool randomWeather { get; set; }
    public bool randomTime { get; set; }
}

public class MGModConfig_Config_LootMultiple
{
    public int Container { get; set; }
    public int Ground { get; set; }
}

public class MGModConfig_Config_Buffs
{
    public bool BuffsWeapon { get; set; }
    public bool BuffsArmor { get; set; }
}

public class MGModConfig_Config_WeatherSettings
{
    public string mode { get; set; }
    public MGModConfig_Config_Weather clouds { get; set; }
    public MGModConfig_Config_Weather windSpeed { get; set; }
    public MinMax<double> rainIntensity { get; set; }
    public MGModConfig_Config_Weather rain { get; set; }
    public MGModConfig_Config_Weather fog { get; set; }
}

public class MGModConfig_Config_Weather
{
    public string type { get; set; }
    public List<double> values { get; set; }
    public List<double> weights { get; set; }
}

public class MGModConfig_Globals
{
    public bool EscapeNoTimeLimit { get; set; }
    public bool FleaMarketOpenLevel { get; set; }
    public bool TakeLimit { get; set; }
    public bool ScavOptimize { get; set; }
    public bool LowTaxRate { get; set; }
    public int SellNumber { get; set; }
    public MGModConfig_Globals_LoadSpeed LoadSpeed { get; set; }
    public bool SuperHero { get; set; }
    public MGModConfig_Globals_LootMultiplier LootMultiplier { get; set; }
    public bool ArmorRepairPerfect { get; set; }
    public MGModConfig_Globals_Buffs Buffs { get; set; }
    public bool ExpOptimize { get; set; }
}
public class MGModConfig_Globals_LoadSpeed
{
    public string mode { get; set; }
    public double BaseLoadTime { get; set; }
    public double BaseUnloadTime { get; set; }
}

public class MGModConfig_Globals_LootMultiplier
{
    public int Value { get; set; }
    public int Global { get; set; }
}

public class MGModConfig_Globals_Buffs
{
    public bool BuffsWeapon { get; set; }
    public bool BuffsArmor { get; set; }
}

public class MGModConfig_Hideout
{
    public EnableValueDouble BuildTime { get; set; }
    public EnableValueDouble ProductTime { get; set; }
    public EnableValueDouble ScavCaseTime { get; set; }
    public bool UpgradeNoLimit { get; set; }
    public EnableValueInt BonusesLevel { get; set; } 
    public bool NoNeedsFuel { get; set; } 
    public MGModConfig_Hideout_Qte Qte { get; set; } 
}

public class MGModConfig_Hideout_Qte
{
    public bool Sucess100 { get; set; }
    public bool NoPunish { get; set; }
    public EnableValueInt RewardMultiple { get; set; }
}

public class MGModConfig_Locations
{
    public EnableValueDouble RaidTime { get; set; } // string || int
    public EnableValueDouble BOSSSpwanChance { get; set; } // string || int
    public bool Pass100 { get; set; }
    public bool Escape100 { get; set; }
    public Dictionary<string, bool> MapInsurance { get; set; }
}

/*
public class MapsBoolean
{
    public bool bigmap { get; set; }
    public bool factory4_day { get; set; }
    public bool factory4_night { get; set; }
    public bool interchange { get; set; }
    public bool laboratory { get; set; }
    public bool lighthouse { get; set; }
    public bool rezervbase { get; set; }
    public bool sandbox { get; set; }
    public bool sandbox_high { get; set; }
    public bool shoreline { get; set; }
    public bool tarkovstreets { get; set; }
    public bool woods { get; set; }
}
*/

public class MGModConfig_Templates
{
    public bool Examined { get; set; }
    public bool WeaponFilter { get; set; }
    public int AmmoStack { get; set; }
    public bool AmmoInfo { get; set; }
    public Dictionary<string, MGModConfig_Templates_ContainerExpands> ContainerExpand { get; set; }
    public MGModConfig_Templates_Safes Safes { get; set; }
    public int MoneyStack { get; set; }
    public MGModConfig_Templates_Backpack Backpack { get; set; }
    public MGModConfig_Templates_ArmorHelmet Armor { get; set; }
    public MGModConfig_Templates_ArmorHelmet Helmet { get; set; }
    public MGModConfig_Templates_EquipmentPlate EquipmentPlate { get; set; }
    public bool KeysDurability { get; set; }
    public int MedcDurability { get; set; }
    public bool WeaponNoLost { get; set; }
    public bool WeaponRepairPerfect { get; set; }
    public int MagazineCapacity { get; set; }
    public bool T7ThermalImaging { get; set; }
    public bool ResetFree { get; set; }
    public bool QuestOptimize { get; set; }
}

public class MGModConfig_Templates_ContainerExpands
{
    public bool enable { get; set; }
    public string Name { get; set; }
    public int cellsH { get; set; }
    public int cellsV { get; set; }
    public bool Weight { get; set; }
    public bool Filter { get; set; }
}

public class MGModConfig_Templates_Safes
{
    public bool SizeExpand { get; set; }
    public bool Filter { get; set; }
    public bool NoWeight { get; set; }
}

public class MGModConfig_Templates_Backpack
{
    public bool Filter { get; set; }
    public bool SmallSize { get; set; }
    public bool NoBuff { get; set; }
    public bool NoWeight { get; set; }

}

public class MGModConfig_Templates_ArmorHelmet
{
    public bool Filter { get; set; }
    // public int Durability { get; set; }
    public bool NoBuff { get; set; }
    public bool NoWeight { get; set; }
}

public class MGModConfig_Templates_EquipmentPlate
{
    public int Durability { get; set; }
    public bool NoBuff { get; set; }
    public bool NoWeight { get; set; }
}
public class MGModConfig_Traders
{
    public EnableValueInt InsuranceTime { get; set; }  // string || int
    public EnableValueDouble InsuranceCost { get; set; }  // string || double
}
public class MGModConfig_MGCustom
{
    public bool CustomTrader { get; set; }
    public bool CustomItem { get; set; }
    public bool CustomAssort { get; set; }
    public bool CustomProfile { get; set; }
    public bool AmmoInfo { get; set; }
    public bool KeyClassfy { get; set; }
    public bool SyncFlea { get; set; }
}

public class EnableValueInt
{
    public bool enable { get; set; }
    public int value { get; set; }
}

public class EnableValueDouble
{
    public bool enable { get; set; }
    public double value { get; set; }
}

public class EnableValueBool
{
    public bool enable { get; set; }
    public bool value { get; set; }
}