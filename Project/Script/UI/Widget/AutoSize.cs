using Godot;

namespace Franken;

public partial class AutoSize : Label
{
    private int maxSize;

    public override void _Ready()
    {
        base._Ready();
        Init();
    }

    public void Init()
    {
        maxSize = LabelSettings.FontSize;
        LabelSettings = LabelSettings.Duplicate() as LabelSettings;
    }

    public void Fit()
    {
        LabelSettings.FontSize = maxSize;
        if (GetLineCount() > GetVisibleLineCount()) Resize();
    }

    private void Resize()
    {
        LabelSettings.FontSize--;
        if (GetLineCount() > GetVisibleLineCount()) Resize();
    }
}
