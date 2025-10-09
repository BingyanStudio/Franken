#if TOOLS
using Godot;

namespace Franken;

public static class AddonsUtil
{
    public const string ADDONS_ROOT = "res://addons";
    public const string WIDGETS_ROOT = $"{ADDONS_ROOT}/widgets";

    public static T CreateWidget<T>() where T : Control => GD.Load<PackedScene>($"{WIDGETS_ROOT}/{typeof(T).Name}.tscn").Instantiate<T>();
}
#endif