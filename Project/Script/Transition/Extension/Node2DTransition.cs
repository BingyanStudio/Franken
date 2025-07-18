using Godot;

namespace Franken;

[TransitionTarget(typeof(Node2D))]
public partial class Node2DTransition : Transition
{
    [TransitionField("Position")] private Vector2 pos;

    private partial void InitPos(Node2D target) => target.Position = pos;

    private partial void ProcessPos(Node2D target, Vector2 delta) => target.Position += delta;
}