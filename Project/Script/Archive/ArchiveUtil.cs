using Godot;
using System.IO;
using System.Text.Json;

namespace Franken;

public static class ArchiveUtil
{
    public static string Root => Path.Combine(OS.GetExecutablePath(), "archive");

    private static readonly JsonSerializerOptions option = new() { WriteIndented = true };

    public static string Serialize<T>(T data) => JsonSerializer.Serialize(data, option);
    public static T Deserialize<T>(string data) => JsonSerializer.Deserialize<T>(data);

    public static void EnsureDirectory(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
    }
}