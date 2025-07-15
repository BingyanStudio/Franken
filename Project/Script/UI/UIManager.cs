using Godot;

namespace Franken;

public partial class UIManager : Singleton<UIManager>
{
    private const string PATH = "res://Assets/Prefab/Window/";

    [Export]
    private Control canvasNormal;
    [Export]
    private Control cacheRoot;

    private static T LoadWindow<T>() where T : UIWindowBase
    {
        var res = ResourceLoader.Load<PackedScene>(PATH + typeof(T).Name + ".tscn");
        if (res == null) return null;

        var window = res.Instantiate<T>();
        if (window == null) return null;

        window.Setup();
        return window;
    }

    private bool TryGetWindow<T>(out T window) where T : UIWindowBase => (window =
        canvasNormal.GetCompInChildren<T>() ?? cacheRoot.GetCompInChildren<T>() ?? LoadWindow<T>()) != null;

    public async void AcquireWindow<T>() where T : UIWindowBase
    {
        if (TryGetWindow(out T window))
        {
            canvasNormal.AddChild(window);
            await window.Show(true);
        }
    }

    public async void RecycleWindow<T>() where T : UIWindowBase
    {
        if (TryGetWindow(out T window))
        {
            await window.Hide(true);
            window.Reparent(cacheRoot);
        }
    }

    public async void DestroyWindow<T>() where T : UIWindowBase
    {
        if (TryGetWindow(out T window))
        {
            await window.Hide(true);
            window.QueueFree();
        }
    }

    public void PreloadWindow<T>() where T : UIWindowBase
    {
        if (TryGetWindow(out T window)) cacheRoot.AddChild(window);
    }
}