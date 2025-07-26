using Godot;
using System.IO;

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
        LogTool.Message("Archive", $"正在保存用户偏好。路径：{path}");
        File.WriteAllText(path, ArchiveUtil.Serialize(Instance));
        LogTool.Message("Archive", $"保存完毕。路径：{path}");
    }

    public static void Load()
    {
        ArchiveUtil.EnsureDirectory(path);

        if (!File.Exists(path))
        {
            LogTool.Warning("Archive", "暂无用户偏好！");
            return;
        }

        LogTool.Message("Archive", $"正在加载用户偏好。路径：{path}");
        Instance = ArchiveUtil.Deserialize<PlayerPref>(path);
        LogTool.Message("Archive", $"加载完毕。路径：{path}");
    }
}