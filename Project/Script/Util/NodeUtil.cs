using Godot;
using System.Linq;

namespace Franken;

public static class NodeUtil
{
    public static T GetCompInChildren<T>(this Node node) where T : Node =>
        node.GetChildren().FirstOrDefault(node => node is T) as T;
}