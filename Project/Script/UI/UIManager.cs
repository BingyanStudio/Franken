using Godot;
using System;
using System.Collections.Generic;

namespace Franken;

public partial class UIManager : Singleton<UIManager>
{
    private const string PATH = "res://Assets/Prefab/UI/Window/";

    [Export]
    private Control canvasNormal;
    [Export]
    private Control cacheRoot;

    private readonly Dictionary<Type, UIWindowBase> activeWindows = [];
    private readonly Dictionary<Type, UIWindowBase> cachedWindows = [];

    private static T LoadWindow<T>() where T : UIWindowBase
    {
        var name = typeof(T).Name;
        var res = ResourceLoader.Load<PackedScene>($"{PATH}{name}/{name}.tscn");
        if (res == null) return null;

        var window = res.Instantiate<T>();
        if (window == null) return null;

        window.Setup();
        return window;
    }

    private bool TryGetWindow<T>(out T window) where T : UIWindowBase => (window =
        (activeWindows.Remove(typeof(T), out var active) ? active as T : null) ??
        (cachedWindows.Remove(typeof(T), out var cached) ? cached as T : null) ??
        LoadWindow<T>()) != null;

    public async void AcquireWindow<T>() where T : UIWindowBase
    {
        if (TryGetWindow(out T window))
        {
            activeWindows.Add(typeof(T), window);
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
            cachedWindows.Add(typeof(T), window);
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
        if (activeWindows.ContainsKey(typeof(T)) || cachedWindows.ContainsKey(typeof(T))) return;

        if (!TryGetWindow(out T window))
        {
            cacheRoot.AddChild(window);
            cachedWindows.Add(typeof(T), window);
        }
    }
}