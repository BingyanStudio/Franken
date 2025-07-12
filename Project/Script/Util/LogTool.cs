using Godot;

namespace Franken;

public static class LogTool
{
    private static string Wrap(string content) => $"[{content}] ";

    public static void Warning(string warning) => GD.PushWarning(warning);

    public static void Warning(string group, string warning) => GD.PushWarning(Wrap(group) + warning);

    public static void Error(string error) => GD.PushError(error);

    public static void Error(string group, string error) => GD.PushError(Wrap(group) + error);

    public static void Message(string message) => GD.Print(message);

    public static void Message(string group, string message) => GD.Print(Wrap(group) + message);

    public static void Raw(string message) => GD.PrintRaw(message);

    public static void Rich(string message) => GD.PrintRich(message);

}