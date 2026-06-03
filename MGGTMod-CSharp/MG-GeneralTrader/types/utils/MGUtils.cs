using _MGGTmod.types.models.Paths;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using SPTarkov.Server.Core.Models.Logging;
using Path = System.IO.Path;

namespace _MGGTmod.types.utils;
[Injectable(TypePriority = OnLoadOrder.PreSptModLoader + 1)]

public class MGUtils(
    ModHelper modHelper,
    JsonUtil jsonUtil,
    FileUtil fileUtil,
    ISptLogger<MGUtils> logger
    )
{
    private string? modPath => modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

    public virtual T? GetJsonDataFromFile<T>(PathType filePath, bool useModPath = true)
    {
        var fullPath = useModPath ? Path.Combine(modPath, filePath.Path) : filePath.Path;
        if (File.Exists(Path.Combine(fullPath, filePath.FileName)))
        {
            return modHelper.GetJsonDataFromFile<T>(fullPath, filePath.FileName);
        }
        return default;
    }

    /// <summary>
    /// 将 source 中非 null 的属性值复制到 target，保留 target 原有的其他属性。
    /// </summary>
    public TemplateItemProperties AssignNonNullProps(TemplateItemProperties source, TemplateItemProperties target)
    {
        if (source == null || target == null) return null;

        foreach (var prop in typeof(TemplateItemProperties).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.CanRead && prop.CanWrite)
            {
                var value = prop.GetValue(source);
                if (value != null)
                {
                    prop.SetValue(target, value);
                }
            }
        }
        return target;
    }

    public string? Serialize<T>(T? obj)
    {
        return jsonUtil.Serialize<T>(obj);
    }

    public T? Deserialize<T>(string? json)
    {
        return jsonUtil.Deserialize<T>(json);
    }

    public bool? HasProp(object obj, string propertyName)
    {
        if (obj == null || string.IsNullOrWhiteSpace(propertyName))
        {
            return false;
        }

        // 获取对象类型
        var type = obj.GetType();

        // 查找属性（忽略大小写）
        var prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (prop == null)
        {
            return false; // 不存在此属性
        }

        var value = prop.GetValue(obj);

        return value != null;
    }

    public bool SetPropValue(object obj, string propertyName, object value)
    {
        if (obj == null || string.IsNullOrWhiteSpace(propertyName))
            return false;

        var type = obj.GetType();
        var prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (prop == null || !prop.CanWrite)
            return false;

        try
        {
            // 获取属性的目标类型（支持 Nullable 类型）
            var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            // 如果值已经是目标类型，就不转换；否则尝试转换
            object convertedValue = value;

            if (value != null && !targetType.IsAssignableFrom(value.GetType()))
            {
                convertedValue = Convert.ChangeType(value, targetType);
            }

            prop.SetValue(obj, convertedValue);
            return true;
        }
        catch
        {
            return false; // 类型不兼容等问题
        }
    }

    public List<string> GetFiles(string relativePath, bool useModePath = true)
    {
        var fullPath = useModePath ? Path.Combine(modPath, relativePath) : relativePath;
        if (Directory.Exists(fullPath))
        {
            var files = fileUtil.GetFiles(fullPath);
            return files;
        }
        return new List<string>();
    }

    public bool DirectoryExists(string relativePath, bool useModePath = true)
    {
        if(useModePath) return fileUtil.DirectoryExists(Path.Combine(modPath, relativePath));
        
        return fileUtil.DirectoryExists(relativePath);
    }
   
    public string[] GetDirectories(string relativePath = "")
    {
        var fullPath = System.IO.Path.Combine(modPath, relativePath);
        if (Directory.Exists(fullPath))
        {
            var directories = fileUtil.GetDirectories(fullPath);
            return directories;
        }
        return Array.Empty<string>();
    }
    
    public bool FileExists(string relativeFilePath)
    {
        return fileUtil.FileExists(System.IO.Path.Combine(modPath, relativeFilePath));
    }
    
    public void DeleteFile(string relativeFilePath)
    {
        fileUtil.DeleteFile(System.IO.Path.Combine(modPath, relativeFilePath));
    }
    
    public void WriteFile(string relativeFilePath, string Data)
    {
        fileUtil.WriteFile(System.IO.Path.Combine(modPath, relativeFilePath), Data);
    }

    public string StripExtension(string filePath)
    {
        return fileUtil.StripExtension(filePath);
    }
    
    public MongoId Generate()
    {
        //return hashUtil.Generate();
        return new MongoId();
    }
    
    public bool IsMongoId(string id)
    {
        //return hashUtil.IsValidMongoId(id);
        return MongoId.IsValidMongoId(id);
    }

    /// <summary>
    /// 替换对象 JSON 中的指定 key，并反序列化为目标类型 T
    /// </summary>
    public T ReplaceKey<T>(T data, string? oldKey, MongoId newKey)
    {
        string dataToString = jsonUtil.Serialize(data, true);
        string replacedData = ReplaceAll(dataToString, oldKey.ToString(), newKey.ToString());
        return jsonUtil.Deserialize<T>(replacedData) ?? throw new Exception("Deserialization failed");
    }

    /// <summary>
    /// 替换字符串中所有出现的旧 key（支持正则安全转义）
    /// </summary>
    private string ReplaceAll(string data, string key, string newKey)
    {
        string escapedKey = EscapeRegex(key);
        return Regex.Replace(data, escapedKey, newKey);
    }

    /// <summary>
    /// 对字符串进行正则表达式安全转义
    /// </summary>
    private string EscapeRegex(string input)
    {
        return Regex.Escape(input);
    }

    public void Log(object data)
    {
        fileUtil.WriteFile(System.IO.Path.Combine(modPath, $"./Log/{new MongoId()}.log"),jsonUtil.Serialize(data));
    }

    public void Log(string server,string data, LogTextColor textColor)
    {
        logger.LogWithColor($"[MGMod][{server}]：" + data, textColor);
    }
    public void Log_GT(string server,string data, LogTextColor textColor)
    {
        logger.LogWithColor($"[MG通用商人框架][{server}]：" + data, textColor);
    }
    public void TestOutput<T>(T data)
    {
        string data2String = jsonUtil.Serialize(data);
        logger.LogWithColor(data2String,  LogTextColor.Gray, LogBackgroundColor.White);
    }
}
