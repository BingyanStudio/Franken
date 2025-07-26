using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Franken;

/// <summary>
/// 存档数据
/// </summary>
public class UserData
{
    [JsonInclude]
    private readonly Dictionary<string, object> data = [];

    public T Get<T>(string key, T fallback)
    {
        if (!data.TryGetValue(key, out object value))
        {
            LogTool.Warning("Archive", $"找不到{key}");
            return fallback;
        }

        return (T)value;
    }

    public void Set<T>(string key, T value) => data[key] = value;
}