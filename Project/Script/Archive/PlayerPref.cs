using Godot;

namespace Franken;

public class PlayerPref : ICustomSerializable
{
    private static readonly string path = ProjectSettings.GlobalizePath("user://pref");

    private static readonly PlayerPref instance = new();

    private float musicVolume = .8f;
    private float soundVolume = .8f;

    public static float MusicVolume
    {
        get => instance.musicVolume;
        set => instance.musicVolume = value;
    }
    public static float SoundVolume
    {
        get => instance.soundVolume;
        set => instance.soundVolume = value;
    }

    public void Serialize(CustomSerializeData data)
    {
        data.Add(musicVolume.ToString());
        data.Add(soundVolume.ToString());
    }

    public void Deserialize(CustomSerializeData data)
    {
        musicVolume = data.Get(.8f);
        soundVolume = data.Get(.8f);
    }

    public static void Save()
    {
        LogTool.Message("Archive", $"正在保存用户偏好。路径：{path}");
        ArchiveUtil.Save(path, instance);
        LogTool.Message("Archive", $"保存完毕。路径：{path}");
    }

    public static void Load()
    {
        LogTool.Message("Archive", $"正在加载用户偏好。路径：{path}");
        ArchiveUtil.Load(path, instance);
        LogTool.Message("Archive", $"加载完毕。路径：{path}");
    }
}