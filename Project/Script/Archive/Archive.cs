using Godot;

namespace Franken;

/// <summary>
/// 存档管理，数据见<see cref="SaveData"/>
/// </summary>
public static class Archive
{
    private static readonly string root = ProjectSettings.GlobalizePath("user://savedata/");

    private static readonly SaveData save = new();

    public static void Save(int idx = 0)
    {
        var path = root + idx.ToString();
        LogTool.Message("Archive", $"开始保存！路径：{path}");
        ArchiveUtil.Save(path, save);
        LogTool.Message("Archive", $"保存完毕！路径：{path}");
    }

    public static void Load(int idx = 0)
    {
        var path = root + idx.ToString();
        LogTool.Message("Archive", $"开始加载存档！路径：{path}");
        ArchiveUtil.Load(path, save);
        LogTool.Message("Archive", $"加载存档完毕！路径：{path}");
    }
}