#if TOOLS
using Godot;

namespace Franken;

[Tool]
public partial class Foldout : Control, IRef
{
    [UIRef]
    private Button title;
    [UIRef]
    private Control content;

    public override void _EnterTree()
    {
        base._EnterTree();

        GetRef();

        title.ToggleMode = true;
        title.Pressed += () => content.Visible = !title.ButtonPressed;;
    }

    public Foldout SetTitle(string target)
    {
        if (title == null) GetRef();
        title.Text = target;
        return this;
    }

    public T AddContent<T>(T target) where T : Control
    {
        if (content == null) GetRef();
        content.AddChild(target);
        return target;
    }
}
#endif