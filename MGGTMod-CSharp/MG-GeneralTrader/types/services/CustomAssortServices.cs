using _MGMod.types.models.EFT.templetes;
using _MGMod.types.models.EFT.traders;
using _MGMod.types.utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Utils;

namespace _MGMod.types.services;
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class CustomAssortServices(
    ISptLogger<CustomAssortServices> logger,
    MGUtils mGUtils
    )
{
   public CustomItemAssorts CreateCustomItemAssorts(MGItem item)
    {
        var customAssorts = new CustomItemAssorts()
        {
            assort = new List<Item>(),
            currency = item.currency?? new MongoId(Money.ROUBLES),
            loyal_level_items = item.loyal_level??1,
            price = item.price,
            traderId = item.toTraderId ?? new MongoId("8ef5b2eff000000000000000")

        };
        if (item.assort.Count > 0)
        {
            customAssorts.assort = FixAssort(item.assort);
        }
        else
        {
            customAssorts.assort = new List<Item>()
            {
                new Item {
                    Id = new MongoId(),
                    Template = item.items.newId,
                    ParentId = "hideout",
                    SlotId = "hideout",
                    Upd = new Upd()
                    {
                        StackObjectsCount = 999999,
                        UnlimitedCount = true,
                    },
                },
            };
        }
        return customAssorts;
    }
    public List<Item> FixAssort(List<Item> assorts)
    {
        var newAssorts = new List<Item>(assorts);
        foreach(var item in newAssorts)
        {
            var oldId = item.Id;
            item.Id = mGUtils.Generate();
            newAssorts = mGUtils.ReplaceKey<List<Item>>(newAssorts, oldId, item.Id);
        }
        return newAssorts;
    }
}
