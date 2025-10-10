#if TOOLS
using Godot;
using System;
using System.Collections.Generic;

namespace Franken;

[Tool]
public partial class LinePopup : Control, IRef
{
    [UIRef]
    private Label label;
    [UIRef]
    private OptionButton option;

    public override void _EnterTree()
    {
        base._EnterTree();

        GetRef();
    }

    public LinePopup SetLabel(string target)
    {
        if (label == null) GetRef();
        label.Text = target;
        return this;
    }

    public LinePopup AddData(string data)
    {
        if (option == null) GetRef();
        option.AddItem(data);
        return this;
    }

    public LinePopup AddData(IEnumerable<string> data)
    {
        if (option == null) GetRef();
        data.ForEach(d => option.AddItem(d));
        return this;
    }

    public LinePopup SetDefault(int idx)
    {
        if (option == null) GetRef();
        option.Select(idx);
        return this;
    }

    public LinePopup OnSelected(Action<string> cbk)
    {
        if (option == null) GetRef();
        option.ItemSelected += idx => cbk?.Invoke(option.GetItemText((int)idx));
        return this;
    }

    public string Read() => option.Text;
}
#endif