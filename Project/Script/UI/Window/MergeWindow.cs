using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Franken;

[ObservableObject]
public partial class MergeWindow : UIWindowBase
{
    private Control partSlots;
    private AnimationPlayer editAnim;
    private Control editCenter;
    private Control editBg;
    private BaseButton bgBtn;
    private BaseButton topBarExitBtn;
    private Label part;
    private BaseButton prev;
    private BaseButton next;

    [ObservableProperty]
    private int currentCompSlotIdx = -1;

    private readonly List<ActorBodyPart.Component> compList = [];
    private ActorBodyPart.Component CurrentComp =>
        CurrentCompSlotIdx < 0 ? ActorBodyPart.Component.None : compList[CurrentCompSlotIdx];

    private void GetRef()
    {
        var children = this.GetAllChildren().ToLookup(child => child.Name);

        partSlots = children["PartSlots"].First() as Control;
        editAnim = children["EditAnim"].First() as AnimationPlayer;
        editCenter = children["EditCenter"].First() as Control;
        editBg = children["EditBg"].First() as Control;
        bgBtn = children["BgBtn"].First() as BaseButton;
        topBarExitBtn = children["TopBarExitBtn"].First() as BaseButton;
        part = children["Part"].First() as Label;
        prev = children["Prev"].First() as BaseButton;
        next = children["Next"].First() as BaseButton;
    }

    private void Bind()
    {
        OnCurrentCompSlotIdxChanged += (oldValue, newValue) =>
        {
            part.Text = CurrentComp.GetDescription();

            if (oldValue < 0)
            {
                editAnim.PlayAnim("Show");
                bgBtn.Visible = true;
            }

            if (newValue < 0)
            {
                editAnim.PlayAnim("Hide");
                bgBtn.Visible = false;
            }
        };

        void UnselectComp() => CurrentCompSlotIdx = -1;
        bgBtn.Pressed += UnselectComp;
        topBarExitBtn.Pressed += UnselectComp;

        void ChangeComp(int offset) => 
            CurrentCompSlotIdx = (compList.Count + CurrentCompSlotIdx + offset) % compList.Count;
        prev.Pressed += () => ChangeComp(-1);
        next.Pressed += () => ChangeComp(1);
    }

    public override void _Ready()
    {
        base._Ready();

        Setup();
    }

    public override void Setup()
    {
        base.Setup();

        GetRef();
        Bind();

        partSlots.TraverseChildren((idx, slot) =>
        {
            var btn = slot.GetChild<BaseButton>(0);

            static string ChangePath(string path) => $"../{path}/Button";
            btn.FocusNeighborTop = ChangePath(slot.FocusNeighborTop);
            btn.FocusNeighborBottom = ChangePath(slot.FocusNeighborBottom);
            btn.FocusNeighborLeft = ChangePath(slot.FocusNeighborLeft);
            btn.FocusNeighborRight = ChangePath(slot.FocusNeighborRight);
            slot.FocusNeighborTop = slot.FocusNeighborBottom = slot.FocusNeighborLeft = slot.FocusNeighborRight = null;

            btn.Pressed += () => CurrentCompSlotIdx = idx;
            compList.Add(Enum.Parse<ActorBodyPart.Component>(slot.GetMeta("Component", "None").AsString()));
        });
    }
}
