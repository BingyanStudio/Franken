using Godot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
    [UIRef]
    private Control bagSlots;
    [UIRef]
    private Control activeSkillsContainer;
    [UIRef]
    private Control passiveSkillsContainer;

    [ObservableProperty("CanZoom")]
    private int currentCompSlotIdx = -1;
    private ObservableCollection<CSV.ActorBodyPart> currentParts = [];

    private bool zooming = false;
    [Export]
    private float zoomTime = .5f;
    [Export]
    private float zoomScale = 1.5f;

    private readonly List<CSV.ActorBodyPart.Component> compList = [];
    private CSV.ActorBodyPart.Component CurrentComp =>
        CurrentCompSlotIdx < 0 ? CSV.ActorBodyPart.Component.None : compList[CurrentCompSlotIdx];

    private CSV.ActorBodyPart[] targetParts;
    private int maxTargetPartsIdx;

    private CSV.ActorBodyPart[] editCache = new CSV.ActorBodyPart[7];

    /// <summary>临时数据源</summary>
    public List<CSV.ActorBodyPart> Parts { get; private set; } = [];

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

                InitBag(Parts);
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

        // 临时背包数据
        CSV.LoadAll();
        CSV.ActorBodyPart.Data.ForEach(data =>
        {
            for (int i = 0; i < 114; i++) Parts.Add(data);
        });
    }

    public override void Setup()
    {
        base.Setup();

        ResetSkillPanel();

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
            compList.Add(Enum.Parse<CSV.ActorBodyPart.Component>(slot.GetMeta("Component", "None").AsString()));
        });
    }

    protected override void Init()
    {
        base.Init();

        CurrentCompSlotIdx = -1;
        editCache = new CSV.ActorBodyPart[7];
        currentParts = [];
        currentParts.CollectionChanged += (sender, args) =>
        {

        };

        RefreshEditSlotFocus(0);
        zooming = false;
    }

    private void RefreshEditSlotFocus(int idx) => partSlots.GetChild(idx).GetNode<BaseButton>("Button").GrabFocus();

    private bool CanZoom(int value) => !zooming;

    private void InitBag(IEnumerable<CSV.ActorBodyPart> data)
    {
        targetParts = data.Where(d => d.Comp.HasFlag(CurrentComp)).ToArray();
        bagSlots.ReserveChildren(targetParts.Length, (idx, slot) =>
        {
            if (slot is not BaseButton btn) return;

            var children = slot.GetAllChildren().ToDictionary(child => child.Name);
            var img = children["Img"] as TextureRect;
            var name = children["Name"] as Label;

            var target = targetParts[idx];

            // TODO: 图片
            name.Text = target.Name;

            if (maxTargetPartsIdx > idx) return;
            maxTargetPartsIdx++;

            btn.FocusEntered += () =>
            {
                var target = targetParts[idx];

                // 技能
                activeSkillsContainer.ReserveChildren(target.Active.Count, (idx, trans) =>
                {
                    var conf = CSV.ActiveConf.Get(target.Active[idx]);
                    if (conf == null)
                    {
                        LogTool.Error("CSV", $"ActiveConf表中找不到主键{target.Active[idx]}");
                        return;
                    }

                    if (trans is not Label label) return;

                    label.Text = $"{conf.Name}：{conf.Desc}";
                });

                passiveSkillsContainer.ReserveChildren(target.Passive.Count, (idx, trans) =>
                {
                    var conf = CSV.PassiveConf.Get(target.Passive[idx]);
                    if (conf == null)
                    {
                        LogTool.Error("CSV", $"PassiveConf表中找不到主键{target.Passive[idx]}");
                        return;
                    }

                    if (trans is not Label label) return;

                    label.Text = $"{conf.Name}：{conf.Desc}";
                });

                // TODO: 数值

                var cache = editCache[CurrentCompSlotIdx];
                if (cache == target) return;

                if (cache != null) currentParts.Remove(cache);
                currentParts.Add(editCache[CurrentCompSlotIdx] = target);
            };

            btn.FocusExited += ResetSkillPanel;
        });
    }

    private void ResetSkillPanel()
    {
        activeSkillsContainer.ReserveChildren(0, null);
        passiveSkillsContainer.ReserveChildren(0, null);
    }
}
