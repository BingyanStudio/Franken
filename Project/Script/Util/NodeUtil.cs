using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Franken;

public static class NodeUtil
{
    public static T GetComponentInChildren<T>(this Node root) where T : Node =>
        root.GetChildren().FirstOrDefault(node => node is T) as T;

    public static IEnumerable<T> GetComponentsInChildren<T>(this Node root) where T : Node => root.GetChildren().OfType<T>();

    public static Node GetComponentByName(this Node root, string name) =>
        root.GetChildren().FirstOrDefault(node => node.Name == name);

    public static T GetComponentByName<T>(this Node root, string name) where T : Node => root.GetComponentByName(name) as T;
}