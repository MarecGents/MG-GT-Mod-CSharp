using _MGMod.types.models.Custom;
using _MGMod.types.models.EFT.traders;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;

namespace _MGMod.types.server;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class TradersServer(
    DatabaseService databaseService
    )
{
    private Dictionary<MongoId, Trader> Traders => databaseService.GetTraders();
    public void AddAssortsToTrader(CustomItemAssorts assorts)
    {
        if (!Traders.ContainsKey(assorts.traderId))
        {
            assorts.traderId = SPTarkov.Server.Core.Models.Enums.Traders.THERAPIST; // 默认是Therapist
		}
        var TraderAssort = Traders[assorts.traderId].Assort;
        TraderAssort.Items.AddRange(assorts.assort);
        var mainAssort = assorts.assort.Find(x => (x.ParentId == "hideout" && x.SlotId == "hideout"));
        var rId = mainAssort.Id;
        TraderAssort.BarterScheme[rId] = [[new BarterScheme() {
            Count = assorts.price,
            Template = assorts.currency
        }]];
        TraderAssort.LoyalLevelItems[rId] = assorts.loyal_level_items;
    }
    public void MGmodTraders(MGModConfig_Traders TradersSetting)
    {
        foreach(var trader in Traders.Keys)
        {
            // if (trader == "ragfair") continue;
            var traderBase = Traders[trader].Base;
            if (traderBase.Insurance.Availability != null && traderBase.Insurance.Availability == true) {
                // 功能：回保速度 InsuranceTime
                if (TradersSetting.InsuranceTime.enable)
                {
                    traderBase.Insurance.MinReturnHour = TradersSetting.InsuranceTime.value;
                    traderBase.Insurance.MaxReturnHour = TradersSetting.InsuranceTime.value * 2;
                }
                // 功能：投保费用 InsuranceCost
                if (TradersSetting.InsuranceCost.enable)
                {
                    foreach(var index in traderBase.LoyaltyLevels)
                    {
                        index.InsurancePriceCoefficient = TradersSetting.InsuranceCost.value * 100;
                    }
                }
            }
        }
    }
}
