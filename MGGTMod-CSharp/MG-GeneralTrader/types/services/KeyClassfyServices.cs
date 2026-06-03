using _MGMod.types.models.Custom;
using _MGMod.types.models.EFT.locales;
using _MGMod.types.models.EFT.templetes;
using _MGMod.types.models.Paths;
using _MGMod.types.server;
using _MGMod.types.utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Utils;

namespace _MGMod.types.services;
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class KeyClassfyServices
{
    private ISptLogger<KeyClassfyServices> logger;
    private LocalesServer  localesServer;
    private TemplatesServer  templatesServer;
    private MGUtils mGUtils;
    
    public KeyClassfyServices(
        ISptLogger<KeyClassfyServices> _logger,
        LocalesServer _localesServer,
        TemplatesServer _templatesServer,
        
        MGUtils  _mGUtils
        )
    {
        logger = _logger;
        localesServer = _localesServer;
        templatesServer = _templatesServer;
        
        mGUtils = _mGUtils;
    }

    public void Start()
    {
        var mapHdIds = AddMapZhNameIdToHb();
        AddorChangeKeyHbParendId(mapHdIds);
        Log("已开启。", LogTextColor.Yellow);
    }

    public Dictionary<string, string> AddMapZhNameIdToHb()
    {
        Dictionary<string,string> mapChName =  mGUtils.GetJsonDataFromFile<Dictionary<string,string>>(Paths.MapChNameJson);
        Dictionary<string,string> mapHdIds = new();
        foreach (var mapName in mapChName)
        {
            MongoId mapHdId = new MongoId();
            mapHdIds.Add(mapName.Key, mapHdId);
            localesServer.AddInfo(new GeneralInfo
            {
                Id = mapHdId,
                Desc =  mapName.Value,
            });
            templatesServer.AddHbCategory(new HandbookCategory
            {
                Id = mapHdId,
                ParentId = "5b47574386f77428ca22b342",
                Icon = "/files/handbook/icon_keys_mechanic.png",
                Color = "",
                Order = "100"
            });
        }

        return mapHdIds;
    }

    public void AddorChangeKeyHbParendId(Dictionary<string, string> mapHdIds)
    {
        HashSet<MongoId> keyCardParendId = ["5c518ec986f7743b68682ce2", "5c518ed586f774119a772aee"];
        Dictionary<string,string> mapChName =  mGUtils.GetJsonDataFromFile<Dictionary<string,string>>(Paths.MapChNameJson);
        Dictionary<string, HashSet<MongoId>> mapKeyList = mGUtils.GetJsonDataFromFile<Dictionary<string, HashSet<MongoId>>>(Paths.MapKeyJson);
        HashSet<MongoId> questKeyList = mGUtils.GetJsonDataFromFile<HashSet<MongoId>>(Paths.QuestKeyJson);
        foreach (var item in templatesServer.GetHandbook().Items)
        {
            if(!keyCardParendId.Contains(item.ParentId)) continue;
            MongoId itemId = item.Id;
            foreach (var (mapName, keyList) in mapKeyList)
            {
                if (!keyList.Contains(itemId)) continue;
                
                if (mapHdIds.Keys.Contains(mapName)) item.ParentId = mapHdIds[mapName];
                else continue;

                ItemsDesc keyInfo = localesServer.GetItemInfoByLang(itemId);
                string Name = $"{keyInfo.Name} {mapChName[mapName]}";
                string Description = $"<color=#00cccc><b>{mapChName[mapName]}</b></color>\r\n{keyInfo.Description}";
                if (questKeyList.Contains(itemId))
                {
                    Name = $"[任务]{Name}";
                    Description = $"<color=#00cccc><b>[任务]</b></color>\r\n{Description}";
                }
                localesServer.SetInfo(new GeneralInfo
                {
                    Id = $"{itemId} Name",
                    Desc = Name,
                });
                localesServer.SetInfo(new GeneralInfo
                {
                    Id = $"{itemId} Description",
                    Desc = Description,
                });
                
            }            
        }
    }

    public void Log(string data, LogTextColor textColor)
    {
        mGUtils.Log("钥匙分类", data, textColor);
    }
}