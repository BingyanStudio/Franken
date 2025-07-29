using Godot;
using System;
using System.Collections.Generic;

namespace Franken;

[ObservableObject]
public partial class MergeWindow : UIWindowBase
{
    [UIRef]
    private Control partSlots;
    [UIRef]
    private AnimationPlayer editAnim;
    [UIRef]
    private Control editCenter;
    [UIRef]
    private Control editBg;
    [UIRef]
    private BaseButton bgBtn;
    [UIRef]
    private BaseButton topBarExitBtn;
    [UIRef]
    private Label part;
    [UIRef]
    private BaseButton prev;
    [UIRef]
    private BaseButton next;

    [ObservableProperty("CanZoom")]
    private int currentCompSlotIdx = -1;

    private bool zooming = false;
    [Export]
    private float zoomTime = .5f;
    [Export]
    private float zoomScale = 1.5f;

    private readonly List<ActorBodyPart.Component> compList = [];
    private ActorBodyPart.Component CurrentComp =>
        CurrentCompSlotIdx < 0 ? ActorBodyPart.Component.None : compList[CurrentCompSlotIdx];

    private void Bind()
    {
        OnCurrentCompSlotIdxChanged += (oldValue, newValue) =>
        {
            zooming = true;
            var zoomTarget = editCenter;
            var zoom = new ControlTransition(editBg);

            if (oldValue < 0)
            {
                editAnim.PlayAnim("Show");
                bgBtn.Visible = true;

                zoom.ToScale(Vector2.One * zoomScale);
            }
            else
            {
            }

            if (newValue < 0)
            {
                editAnim.PlayAnim("Hide");
                bgBtn.Visible = false;

                zoom.ToScale(Vector2.One);

                RefreshEditSlotFocus(oldValue);
            }
            else
            {
                zoomTarget = partSlots.GetChild<Control>(newValue);
                part.Text = CurrentComp.GetDescription();
            }

            zoom.ToPos(editCenter.Position - zoomTarget.Position)
                .ToPivotOffset(zoomTarget.Position);

            Flow.Create()
                .Delay(zoom.During(zoomTime).Easing(Transition.OutCubic))
                .Then(() => zooming = false)
                .Run();
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
        Init();
    }

    public override void Setup()
    {
        base.Setup();

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

    protected override void Init()
    {
        base.Init();
        currentCompSlotIdx = -1;
        RefreshEditSlotFocus(0);
        zooming = false;
    }

    private void RefreshEditSlotFocus(int idx) => partSlots.GetChild(idx).GetNode<BaseButton>("Button").GrabFocus();

    private bool CanZoom(int value) => !zooming;
}
