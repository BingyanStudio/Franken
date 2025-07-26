using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Franken;

public static class NodeUtil
{
    public static List<Node> GetAllChildren(this Node node)
    {
        List<Node> children = [.. node.GetChildren()];
        for (int i = 0; i < children.Count; i++) children.AddRange(children[i].GetChildren());
        return children;
    }

    public static T GetComponentInChildren<T>(this Node root) where T : Node =>
        root.GetChildren().FirstOrDefault(node => node is T) as T;

    public static IEnumerable<T> GetComponentsInChildren<T>(this Node root) where T : Node => root.GetChildren().OfType<T>();

    public static Node Root => (Engine.GetMainLoop() as SceneTree).Root.GetChildren().Last();
}