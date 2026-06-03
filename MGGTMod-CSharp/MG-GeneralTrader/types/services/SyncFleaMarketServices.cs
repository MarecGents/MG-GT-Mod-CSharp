using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Reflection;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;

using _MGMod.types.models.Paths;
using _MGMod.types.models.EFT.templetes;
using _MGMod.types.server;
using _MGMod.types.utils;
using SPTarkov.Server.Core.Models.Utils;
namespace _MGMod.types.services;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class SyncFleaMarketServices
{

    private PriceType? priceJson;
    private GitHubTokenType? githubToken;
    private ISptLogger<SyncFleaMarketServices> logger;
    private MGUtils mGUtils;
    private DatabaseService databaseService;
    private ConfigsServer  configsServer;
    
    public SyncFleaMarketServices(
        ISptLogger<SyncFleaMarketServices> _logger,
        DatabaseService _databaseService,
        ConfigsServer _configsServer,
        MGUtils _mGUtils
        )
    {
        logger = _logger;
        databaseService = _databaseService;
        configsServer = _configsServer;
        mGUtils = _mGUtils;
    }

    public async Task Start()
    {
        configsServer.ApplyBaseFleaPrices();
        await Init();
    }

    public async Task Init()
    {
        if (!mGUtils.FileExists(Path.Combine(Paths.PriceJson.Path, Paths.PriceJson.FileName)))
        {
            DateTime date = (DateTime.Now).AddDays(-4);
            priceJson = new PriceType { date = [date.Year, date.Month, date.Day], prices = databaseService.GetPrices() };
        }
        else
        {
            priceJson = mGUtils.GetJsonDataFromFile<PriceType>(Paths.PriceJson);
        }

        if (!mGUtils.FileExists(Path.Combine(Paths.GithubToken.Path, Paths.GithubToken.FileName)))
        {
            return;
        }

        githubToken = mGUtils.GetJsonDataFromFile<GitHubTokenType>(Paths.GithubToken);
        
        DateTime nowDate = new DateTime(priceJson.date[0], priceJson.date[1], priceJson.date[2]);
        TimeSpan diff = DateTime.Now - nowDate;
        if (diff.TotalDays < 3) LoadPrice();
        else
        {
            Log("同步数据与当前日期差距过大，正在重新同步。", LogTextColor.Blue);
            await GetPrices();
            LoadPrice();
        }
    }

    private async Task GetPrices()
    {
        if (githubToken == null) return;

        var client = new HttpClient();

        // GitHub API 要求带有 User-Agent
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MyApp", "1.0"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", githubToken.token);

        string url = $"https://api.github.com/repos/{githubToken.owner}/{githubToken.repo}/contents/{githubToken.filePath}";

        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            // GitHub 的 contents API 返回的是 Base64 编码的内容
            var jsonDoc = JsonDocument.Parse(content);
            var encodedContent = jsonDoc.RootElement.GetProperty("content").GetString();
            var decodedBytes = Convert.FromBase64String(encodedContent.Replace("\n", ""));
            string fileContent = Encoding.UTF8.GetString(decodedBytes);

            priceJson = mGUtils.Deserialize<PriceType>(fileContent);
            SavePrice();
        }
        catch (Exception ex)
        {
            Log($"获取出错<{ex.Message}>。", LogTextColor.Red);
        }

    }
    private void SavePrice()
    {
        if (priceJson == null) return;
        mGUtils.WriteFile(Path.Combine(Paths.PriceJson.Path, Paths.PriceJson.FileName), mGUtils.Serialize(priceJson));
    }

    private void LoadPrice()
    { 
        if (priceJson == null) return;
        var prices = databaseService.GetPrices();
        foreach (var id in prices.Keys)
        {
            if (priceJson.prices.TryGetValue(id, out var price))
            {
                prices[id] = price;
            }
        }

        // var HbItem = databaseService.GetHandbook().Items;
        // foreach (var item in HbItem)
        // {
        //     if (priceJson.prices.TryGetValue(item.Id, out var price))
        //     {
        //         item.Price = price;
        //     }
        // }
		Log($"已同步至日期 {priceJson.date[0]}年{priceJson.date[1]}月{priceJson.date[2]}日。", LogTextColor.Cyan);
    }
    
    private void Log(string data, LogTextColor textColor)
    {
        mGUtils.Log("实时跳蚤", data, textColor);
    }
}

public class GitHubTokenType
{
    public required string token { get; set; }
    public required string owner { get; set; }
    public required string repo { get; set; }
    public required string filePath { get; set; }
}
