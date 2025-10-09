using Godot;
using System.IO;
using Godotool;

namespace Franken;

/// <summary>
/// 用户偏好，全局数据
/// </summary>
public class PlayerPref
{
    private static readonly string path = Path.Combine(ArchiveUtil.Root, "pref.json");

    public static PlayerPref Instance { get; private set; } = new();

    public float MusicVolume { get; set; } = .8f;
    public float SoundVolume { get; set; } = .8f;

    public static void Save()
    {
        ArchiveUtil.EnsureDirectory(path);
        Log.I("Archive", $"正在保存用户偏好。路径：{path}");
        File.WriteAllText(path, ArchiveUtil.Serialize(Instance));
        Log.I("Archive", $"保存完毕。路径：{path}");
    }

    public static void Load()
    {
        ArchiveUtil.EnsureDirectory(path);

        if (!File.Exists(path))
        {
            Log.W("Archive", "暂无用户偏好！");
            return;
        }

        Log.I("Archive", $"正在加载用户偏好。路径：{path}");
        Instance = ArchiveUtil.Deserialize<PlayerPref>(path);
        Log.I("Archive", $"加载完毕。路径：{path}");
    }
}