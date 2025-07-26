using Godot;
using System.Linq;

namespace Franken;

public partial class MergeWindow : UIWindowBase
{
    private Control partSlots;

    public override void _Ready()
    {
        base._Ready();

        Setup();
    }

    public override void Setup()
    {
        base.Setup();

        var children = this.GetAllChildren().ToLookup(child => child.Name);

        partSlots = children["PartSlots"].First() as Control;
        partSlots.TraverseChildren(slot =>
        {
            var btn = slot.GetChild<Control>(0);
            btn.FocusNeighborTop = $"../{slot.FocusNeighborTop}/Button";
            btn.FocusNeighborBottom = $"../{slot.FocusNeighborBottom}/Button";
            btn.FocusNeighborLeft = $"../{slot.FocusNeighborLeft}/Button";
            btn.FocusNeighborRight = $"../{slot.FocusNeighborRight}/Button";
            slot.FocusNeighborTop = slot.FocusNeighborBottom = slot.FocusNeighborLeft = slot.FocusNeighborRight = null;
        });
    }
}
