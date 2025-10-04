using Godot;
using System;

namespace Franken;

public partial class DraggablePanel : Control
{
    private bool _isDragging = false;

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            _isDragging = mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left;
        }

        if (@event is InputEventMouseMotion mouseMotion && _isDragging)
        {
            Position += mouseMotion.Relative;
        }
    }
}
