namespace Franken;

public partial class MergeWindow : UIWindowBase
{
    public override void _Ready()
    {
        base._Ready();
        Flow.Create().Delay(1).Then(() => LogTool.Message("111")).Run();
    }
}