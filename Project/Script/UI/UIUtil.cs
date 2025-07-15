using Godot;
using System;

namespace Franken;

public static class UIUtil
{
    public static void TraverseChildren(this Control root, Action<Control> action)
    {
        foreach (var control in root.GetChildren()) action.Invoke(control as Control);
    }

    public static void TraverseChildren(this Control root, Action<int, Control> action)
    {
        var children = root.GetChildren();
        for (var i = 0; i < children.Count; i++) action.Invoke(i, children[i] as Control);
    }
}