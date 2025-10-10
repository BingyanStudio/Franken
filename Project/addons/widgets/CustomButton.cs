#if TOOLS
using Godot;
using System;

namespace Franken;

[Tool]
public partial class CustomButton : Button
{
    public CustomButton SetLabel(string label)
    {
        Text = label;
        return this;
    }

    public CustomButton OnPressed(Action cbk)
    {
        Pressed += cbk;
        return this;
    }
}
#endif