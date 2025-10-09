using System.Collections.Generic;
using System.Text.Json.Serialization;
using Godotool;

namespace Franken;

/// <summary>
/// 存档数据
/// </summary>
public class UserData
{
    [JsonInclude]
    private readonly Dictionary<string, string> data = [];

    public T Get<T>(string key, T fallback)
    {
        if (!data.TryGetValue(key, out var value))
        {
            Log.W("Archive", $"找不到{key}");
            return fallback;
        }

        return ArchiveUtil.Deserialize<T>(value);
    }

    public void Set<T>(string key, T value) => data[key] = ArchiveUtil.Serialize(value);
}