using Godot;

namespace Franken;

public partial class SubstancePTest : Node
{
    public override void _Ready()
    {
        base._Ready();

        UIManager.Instance.AcquireWindow<MergeWindow>();
    }
}