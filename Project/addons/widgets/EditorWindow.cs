#if TOOLS
using Godot;

namespace Franken;

[Tool]
public partial class EditorWindow : Control, IRef
{
    [UIRef]
    private VBoxContainer content;

    public override void _EnterTree()
    {
        base._EnterTree();

        GetRef();
    }

    public T AddContent<T>(T target) where T : Control
    {
        if (content == null) GetRef();
        content.AddChild(target);
        return target;
    }
}
#endif