#if TOOLS
using Godot;

namespace Franken;

[Tool]
public partial class LineInput : Control, IRef
{
    [UIRef]
    private Label label;
    [UIRef]
    private LineEdit input;

    public override void _EnterTree()
    {
        base._EnterTree();

        GetRef();
    }

    public LineInput SetLabel(string target)
    {
        if (label == null) GetRef();
        label.Text = target;
        return this;
    }

    public string Read() => input.Text;
}
#endif