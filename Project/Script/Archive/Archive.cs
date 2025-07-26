using System.IO;

namespace Franken;

/// <summary>
/// 存档管理，局部见<see cref="UserData"/>，全局见<see cref="PlayerPref"/>
/// </summary>
public static class Archive
{
    public const int DATA_COUNT = 10;

    public static readonly string root = Path.Combine(ArchiveUtil.Root, "data");

    public static UserData[] Data { get; private set; } = new UserData[DATA_COUNT];

    private static int current = 0;
    public static int Current
    {
        get => current;
        set
        {
            if (value < 0 || value >= Data.Length) LogTool.Error("Archive", $"存档序号{value}超出范围！");
            else current = value;
        }
    }

    public static void Save() => Save(Current);
    public static void Save(int idx)
    {
        var path = FindPath(idx);
        ArchiveUtil.EnsureDirectory(path);

        LogTool.Message("Archive", $"开始保存！路径：{path}");
        File.WriteAllText(path, ArchiveUtil.Serialize(Data[idx]));
        LogTool.Message("Archive", $"保存完毕！路径：{path}");
    }

    public static void Load() => Load(Current);
    public static void Load(int idx)
    {
        var path = FindPath(idx);
        ArchiveUtil.EnsureDirectory(path);

        if (!File.Exists(path))
        {
            LogTool.Warning("Archive", $"无{idx}号存档。");
            return;
        }

        LogTool.Message("Archive", $"开始加载存档！路径：{path}");
        Data[idx] = ArchiveUtil.Deserialize<UserData>(File.ReadAllText(path));
        LogTool.Message("Archive", $"加载存档完毕！路径：{path}");
    }

    public static void LoadAll() => Data.Traverse((idx, _) => Load(idx));

    private static string FindPath(int idx) => Path.Combine(root, $"{idx}.json");
}