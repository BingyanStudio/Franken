namespace Franken;

public static class ArchiveUtil
{
    public static void Save<T>(string path, T target) where T : ICustomSerializable
    {
        using var data = new CustomSerializeData();
        target.Serialize(data);
        data.Save(path);
    }

    public static void Load<T>(string path, T target) where T : ICustomSerializable
    {
        using var data = new CustomSerializeData();
        data.Load(path);
        target.Deserialize(data);
    }
}