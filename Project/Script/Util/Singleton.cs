using Godot;

namespace Franken;

public abstract partial class Singleton<T> : Node where T : Singleton<T>
{
    public static T Instance { get; private set; }

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this as T;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Instance = null;
    }
}