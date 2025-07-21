using Godot;

namespace Franken;

[TransitionTarget(typeof(Control))]
public partial class ControlTransition : Transition
{
    [TransitionField("Position")] private Vector2 pos;

    private partial void InitPos(Control target) => target.Position = pos;

    private partial void ProcessPos(Control target, Vector2 delta) => target.Position += delta;
}