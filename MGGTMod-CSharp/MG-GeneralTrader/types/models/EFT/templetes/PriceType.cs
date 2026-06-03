using SPTarkov.Server.Core.Models.Common;

namespace _MGMod.types.models.EFT.templetes;

public class PriceType
{
    public required List<int> date {  get; set; }
    public required Dictionary<MongoId,double> prices { get; set; }
}
