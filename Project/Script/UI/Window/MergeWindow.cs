using Godot;

namespace Franken;

[ObservableObject]
public partial class MergeWindow : UIWindowBase
{
    [ObservableProperty(Validate = "Foo")]
    private int idx = 0;

    private bool Foo(int x) => x >= 0;

    public override void _Ready()
    {
        base._Ready();
        Idx = -1;
        LogTool.Message(Idx.ToString());
        Idx = 1;
        LogTool.Message(Idx.ToString());
        Flow.Create().Delay(1).Delay(this.AddPos(Vector2.Right * 114).During(1).Easing(Transition.InCubic)).Delay(this.FromPos(Vector2.Right * 114).ToPos(Vector2.Right * 114 + Vector2.Down * 514).During(1).Easing(Transition.InCubic)).Run();
    }
}
