using Godot;

namespace Franken;

[TransitionTarget(typeof(Control))]
public partial class ControlTransition : Transition
{
    [TransitionField("Position")] private Vector2 pos;
    private partial void InitPos(Control target) => target.Position = pos;
    private partial void ProcessPos(Control target, Vector2 delta) => target.Position += delta;

    [TransitionField("Scale")] private Vector2 scale;
    private partial void InitScale(Control target) => target.Scale = scale;
    private partial void ProcessScale(Control target, Vector2 delta) => target.Scale += delta;

    [TransitionField("PivotOffset")] private Vector2 pivotOffset;
    private partial void InitPivotOffset(Control target) => target.PivotOffset = pivotOffset;
    private partial void ProcessPivotOffset(Control target, Vector2 delta) => target.PivotOffset += delta;
}