using Godot;
using Godotool;

namespace Franken;

/// <summary>
/// 存档管理，局部见<see cref="UserData"/>，全局见<see cref="PlayerPref"/>
/// </summary>
public static class Archive
{
    public const int DATA_COUNT = 10;

    public const string ROOT = $"{ArchiveUtil.ROOT}/data";

    public static UserData[] Data { get; private set; } = new UserData[DATA_COUNT];

    private static int current = 0;
    public static int Current
    {
        get => current;
        set
        {
            if (value < 0 || value >= Data.Length) Log.E($"存档序号{value}超出范围！");
            else current = value;
        }
    }

    public static void Save() => Save(Current);
    public static void Save(int idx)
    {
        ArchiveUtil.EnsureDirectory(ROOT);

        var path = FindPath(idx);

        Log.I($"开始保存！路径：{path}");
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        file.StoreString(ArchiveUtil.Serialize(Data[idx]));
        Log.I($"保存完毕！路径：{path}");
    }

    public static void Load() => Load(Current);
    public static void Load(int idx)
    {
        ArchiveUtil.EnsureDirectory(ROOT);

        var path = FindPath(idx);
        if (!FileAccess.FileExists(path))
        {
            using var create = FileAccess.Open(path, FileAccess.ModeFlags.Write);
            create.StoreString("{}");
        }

        Log.I($"开始加载存档！路径：{path}");
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        Data[idx] = ArchiveUtil.Deserialize<UserData>(file.GetAsText());
        Log.I($"加载存档完毕！路径：{path}");
    }

    public static void Delete(int idx)
    {
        var path = FindPath(idx);
        if (FileAccess.FileExists(path))
        {
            Log.I($"删除存档{idx}，路径：{path}");
            DirAccess.RemoveAbsolute(path);
        }
        else Log.W($"无{idx}号存档，路径：{path}");
    }

    public static void LoadAll() => Data.Traverse((idx, _) => Load(idx));

    private static string FindPath(int idx) => $"{ROOT}/{idx}.json";
}
