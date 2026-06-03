using _MGGTmod.types.models.Custom;
using _MGGTmod.types.models.EFT.traders;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;

namespace _MGGTmod.types.server;

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
}
