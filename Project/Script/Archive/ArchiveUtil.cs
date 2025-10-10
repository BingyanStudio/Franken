using Godot;
using System.Text.Json;

namespace Franken;

public static class ArchiveUtil
{
    public const string ROOT = "user://archive";

    private static readonly JsonSerializerOptions option = new() { WriteIndented = true };

    public static string Serialize<T>(T data) => JsonSerializer.Serialize(data, option);
    public static T Deserialize<T>(string data) => JsonSerializer.Deserialize<T>(data);

    public static void EnsureDirectory(string path) => DirAccess.MakeDirRecursiveAbsolute(path);
}