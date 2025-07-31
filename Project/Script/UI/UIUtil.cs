using Godot;
using System;
using System.Linq;

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

    public static void ReserveChildren(this Control root, int count, Action<int, Control> action)
    {
        if (root.GetChild<Control>(0) is not Control child) return;
        var children = root.GetChildren().OfType<Control>().ToList();

        if (count < children.Count)
            for (int i = count; i < children.Count; i++)
                children[i].Visible = false;

        if (count > children.Count)
            for (int i = children.Count; i < count; i++)
            {
                var copy = child.Duplicate() as Control;
                root.AddChild(copy);
                children.Add(copy);
            }

        for (var i = 0; i < count; i++)
        {
            var target = children[i];
            target.Visible = true;
            action?.Invoke(i, target);
        }
    }
}