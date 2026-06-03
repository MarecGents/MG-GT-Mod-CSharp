using SPTarkov.Server.Core.Models.Spt.Server;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Services;

using _MGGTmod.types.models.EFT.locales;
namespace _MGGTmod.types.server;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class LocalesServer(
    DatabaseService databaseService
    )
{
    private LocaleBase Locale => databaseService.GetLocales();
    
    private List<string> _itemDescList = new () { "Name", "ShortName", "Description" };
    private List<string> _questDescList = new () 
    {
        "name",
        "description",
        "failMessageText",
        "successMessageText",
        "acceptPlayerMessage",
        "declinePlayerMessage",
        "completePlayerMessage"
    };
    private List<string> _traderDescList = new () 
    {
        "FullName",
        "FirstName",
        "Nickname",
        "Location",
        "Description"
    };
    
    public LocaleBase GetLocales()
    {
        var locale = Locale;
        return locale;
    }

    public string GetLocale(string lang)
    {
        var global = Locale.Global[lang].Value;

        if (global != null) return global[lang];
        
        return "null";
    }
    
    public string GetInfoByKey(string key, string lang = "ch")
    {
        var global = Locale.Global[lang].Value;
        if (global.ContainsKey(key))
        {
            return global[key];
        }
        return "";
    }
    
    public ItemsDesc GetItemInfoByLang(string id, string lang = "ch")
    {
        var global = Locale.Global[lang].Value;
        
        ItemsDesc itemsDesc = new()
        {
            Name = "",
            ShortName = "",
            Description = "",
        };

        foreach (var key in _itemDescList)
        {
            string keyDesc = $"{id} {key}";
            if(!global.ContainsKey(keyDesc)) continue;
            //itemsDesc[key] = global[keyDesc];
            // 通过反射给 itemsDesc 对应属性赋值
            var prop = typeof(ItemsDesc).GetProperty(key);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(itemsDesc, global[keyDesc]);
            }
        }
        
        return itemsDesc;
    }

    public string GetTraderNicknameByLang(string id, string lang="ch")
    {
        var global = Locale.Global[lang].Value;

        if (global.ContainsKey($"{id} Nickname"))
        {
            return global[$"{id} Nickname"];
        }
        
        return "";                                 
    }

    public void SetInfo(GeneralInfo info)
    {
        var global = Locale.Global;

        foreach (var lang in global.Keys)
        {
            
            if (global.TryGetValue(lang, out var lazyLoad))
            {
                lazyLoad.AddTransformer(localeData =>
                {
                    if (!localeData.ContainsKey(info.Id)) return localeData;
                    localeData[info.Id] = info.Desc;
                    return localeData;
                });
            }
        }
    }

    public void AddInfo(GeneralInfo info)
    {
        var global = Locale.Global;
        
        foreach (var lang in global.Keys) 
        {
            if(global.TryGetValue(lang, out var lazyLoad))
            {
                lazyLoad.AddTransformer(localeData =>
                {
                    if (localeData.ContainsKey(info.Id)) return localeData;
                    localeData[info.Id] = info.Desc;
                    return localeData;
                });
            }
        }
    }
    
    public void AddItemInfo(ItemsInfo info)
    {
        var global = Locale.Global;

        foreach (var lang in global.Keys)
        {
            foreach (var desc in _itemDescList)
            {
                if (global.TryGetValue(lang, out var lazyLoad))
                {
                    lazyLoad.AddTransformer(localeData =>
                    {
                        string key = $"{info.Id} {desc}";
                        if (localeData.ContainsKey($"{info.Id} {desc}")) return localeData;
                        var prop = info.Desc.GetType().GetProperty(desc);
                        if (prop != null)
                        {
                            var value = prop.GetValue(info.Desc)?.ToString() ?? "";
                            localeData[key] = value;
                        }
                        return localeData;
                    });
                }
            }
        }
    }

    public void AddQuestInfo(QuestInfo info)
    {
        var global = Locale.Global;
        
        foreach(var lang  in global.Keys)
        {
            foreach (var desc in _questDescList)
            {
                if (global.TryGetValue(lang, out var lazyLoad))
                {
                    lazyLoad.AddTransformer(localeData =>
                    {
                        string key = $"{info.Id} {desc}";
                        if (localeData.ContainsKey($"{info.Id} {desc}")) return localeData;
                        var prop = info.Desc.GetType().GetProperty(desc);
                        if (prop != null)
                        {
                            var value = prop.GetValue(info.Desc)?.ToString() ?? "";
                            localeData[key] = value;
                        }
                        return localeData;
                    });
                }
            }
            if(info.Desc.other != null && info.Desc.other.Keys.Count != 0)
            {
                foreach(var key in info.Desc.other.Keys)
                {
                    if (global.TryGetValue(lang, out var lazyLoad))
                    {
                        lazyLoad.AddTransformer(localeData =>
                        {
                            if (localeData.ContainsKey(key)) return localeData;
                            localeData[key] = info.Desc.other[key];
                            return localeData;
                        });
                    }
                }
            }
        }
    }

    public void AddTraderInfo(TraderInfo info)
    {
        var global = Locale.Global;

        foreach (var lang in global.Keys)
        {
            foreach (var desc in _traderDescList)
            {
                if (global.TryGetValue(lang, out var lazyLoad))
                {
                    lazyLoad.AddTransformer(localeData =>
                    {
                        string key = $"{info.Id} {desc}";
                        if (localeData.ContainsKey($"{info.Id} {desc}")) return localeData;
                        var prop = info.Desc.GetType().GetProperty(desc);
                        if (prop != null)
                        {
                            var value = prop.GetValue(info.Desc)?.ToString() ?? "";
                            localeData[key] = value;
                        }
                        return localeData;
                    });
                }
            }
        }
    }

    public void AddProfileInfo(AnyInfo info)
    {
        var global = Locale.Global;
        
    }
    
}