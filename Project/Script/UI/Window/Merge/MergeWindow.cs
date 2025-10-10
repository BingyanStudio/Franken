using Godot;
using Godotool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Franken;

[ObservableObject]
public partial class MergeWindow : UIWindowBase, IRef
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
    private BaseButton bagBarExitBtn;
    [UIRef]
    private Label part;
    [UIRef]
    private BaseButton prev;
    [UIRef]
    private BaseButton next;
    [UIRef]
    private Control bagSlots;
    #region 技能
    [UIRef]
    private Control activeSkillsContainer;
    [UIRef]
    private Control passiveSkillsContainer;
    #endregion
    #region 数值
    [UIRef]
    private Control hpBar;
    [UIRef]
    private Control defBar;
    [UIRef]
    private Control agiBar;
    [UIRef]
    private Control sanBar;
    [UIRef]
    private Control cmpBar;
    [UIRef]
    private Control ptiBar;
    [UIRef]
    private Control pthBar;
    #endregion
    [UIRef]
    private LineEdit inputActorName;
    [UIRef]
    private BaseButton confirmBtn;

    [ObservableProperty("CanZoom")]
    private int currentCompSlotIdx = -1;
    private ObservableCollection<ActorBodyPartSlot> currentParts = [];

    private bool zooming = false;
    [Export]
    private float zoomTime = .5f;
    [Export]
    private float zoomScale = 1.5f;

    private readonly List<CSV.ActorBodyPart.Component> compList = [];
    private CSV.ActorBodyPart.Component CurrentComp =>
        CurrentCompSlotIdx < 0 ? CSV.ActorBodyPart.Component.None : compList[CurrentCompSlotIdx];

    private ActorBodyPartSlot[] targetParts;
    private int maxTargetPartsIdx;

    private ActorBodyPartSlot[] editCache = new ActorBodyPartSlot[7];

    public List<ActorBodyPartSlot> Parts { get; private set; } = [];

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

                RefreshSkillsAndStats();
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
        bagBarExitBtn.Pressed += UnselectComp;

        void ChangeComp(int offset) =>
            CurrentCompSlotIdx = (compList.Count + CurrentCompSlotIdx + offset) % compList.Count;
        prev.Pressed += () => ChangeComp(-1);
        next.Pressed += () => ChangeComp(1);

        confirmBtn.Pressed += OnConfirm;
    }

    public override void Setup()
    {
        base.Setup();

        GetRef();

        // 暂时在此加载存档
        Archive.Load();

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

            btn.MouseEntered += () => FocusOnItem(editCache[idx]);
            btn.MouseExited += RefreshSkillsAndStats;

            btn.FocusEntered += () => FocusOnItem(editCache[idx]);
            btn.FocusExited += RefreshSkillsAndStats;

            compList.Add(Enum.Parse<CSV.ActorBodyPart.Component>(slot.GetMeta("Component", "None").AsString()));
        });
    }

    protected override void Init()
    {
        base.Init();

        Reset();

        CurrentCompSlotIdx = -1;
        editCache = new ActorBodyPartSlot[7];
        currentParts = [];
        currentParts.CollectionChanged += (sender, args) =>
        {

        };

        RefreshEditSlotFocus(0);
        zooming = false;
    }

    private void RefreshEditSlotFocus(int idx) => partSlots.GetChild(idx).GetNode<BaseButton>("Button").GrabFocus();

    // 不要被代码提示骗了，它不懂源生
    private bool CanZoom(int value) => !zooming;

    private void InitBag(IEnumerable<ActorBodyPartSlot> data)
    {
        targetParts = data.Where(d => d.Item.Comp.HasFlag(CurrentComp)).ToArray();
        bagSlots.ReserveChildren(targetParts.Length, (idx, slot) =>
        {
            if (slot is not BaseButton btn) return;

            var part = targetParts[idx];
            SetSlot(slot, part);

            btn.Disabled = part.Selected && editCache[CurrentCompSlotIdx] != part;
            if (part.Selected && editCache[currentCompSlotIdx] == part) btn.GrabFocus();

            if (maxTargetPartsIdx > idx) return;
            maxTargetPartsIdx++;

            btn.Pressed += () => SelectBagItem(idx);

            btn.MouseEntered += () => FocusOnBagItem(idx);
            btn.MouseExited += RefreshSkillsAndStats;

            btn.FocusEntered += () => FocusOnBagItem(idx);
            btn.FocusExited += RefreshSkillsAndStats;
        });
    }

    private void SelectBagItem(int idx)
    {
        var part = targetParts[idx];
        var cache = editCache[CurrentCompSlotIdx];
        var slot = partSlots.GetChild(CurrentCompSlotIdx) as Control;

        if (cache != null)
        {
            cache.Selected = false;
            currentParts.Remove(cache);

            if (cache == part)
            {
                SetSlot(slot, editCache[CurrentCompSlotIdx] = null);
                return;
            }
        }

        part.Selected = true;
        currentParts.Add(editCache[CurrentCompSlotIdx] = part);
        SetSlot(slot, part);
    }

    private static void SetSlot(Control slot, ActorBodyPartSlot part)
    {
        var children = slot.GetAllChildren().ToDictionary(child => child.Name);
        var img = children["Img"] as TextureRect;
        var name = children["Name"] as Label;

        if (part == null)
        {
            name.Text = string.Empty;
            return;
        }

        // TODO: 图片
        name.Text = part.Name;
        if (name is AutoSize autoSize) autoSize.Fit();
    }

    private void FocusOnBagItem(int idx) => FocusOnItem(targetParts[idx]);

    private void FocusOnItem(ActorBodyPartSlot part)
    {
        if (part == null) return;

        RefreshStats(ActorBodyPartStats.FromCSV(part.Item.ID));
        RefreshSkills(
            part.Item.Active.Select(CSV.ActiveConf.Get).ToArray(),
            part.Item.Passive.Select(CSV.PassiveConf.Get).ToArray());
    }

    private void PrepareData()
    {
        Parts.Clear();
        // 背包数据
        UserData.Current.ActorBodyParts?.ForEach(data => Parts.Add(new(CSV.ActorBodyPart.Get(data))));
        // 临时数据
        if (Parts.Count == 0) CSV.ActorBodyPart.Data.ForEach(data =>
            IEnumerableUtil.Range(0, 10).ForEach(_ => Parts.Add(new(data))));
    }

    private void ResetSkillPanel()
    {
        activeSkillsContainer.ReserveChildren(0, null);
        passiveSkillsContainer.ReserveChildren(0, null);
    }

    private static void SetValue(Control bar, string value)
    {
        if (bar.GetNode("Value") is Label label) label.Text = value;
    }

    private void RefreshSkills(CSV.ActiveConf[] actives, CSV.PassiveConf[] passives)
    {
        activeSkillsContainer.ReserveChildren(actives.Length, (idx, trans) =>
        {
            if (trans is not Label label) return;

            var conf = actives[idx];
            label.Text = $"{conf.Name}：{conf.Desc}";
        });
        passiveSkillsContainer.ReserveChildren(passives.Length, (idx, trans) =>
        {
            if (trans is not Label label) return;

            var conf = passives[idx];
            label.Text = $"{conf.Name}：{conf.Desc}";
        });
    }

    private void RefreshStats(ActorBodyPartStats stats)
    {
        static string Number2String(Number number) => number.Type switch
        {
            Number.ValueType.Int => ((int)number).ToString(),
            Number.ValueType.Float => ((int)((float)number * 100)).ToString() + "%",
            _ => "0"
        };

        SetValue(hpBar, Number2String(stats.Hp));
        SetValue(defBar, Number2String(stats.Def));
        SetValue(agiBar, Number2String(stats.Agi));
        SetValue(sanBar, Number2String(stats.San));
        SetValue(cmpBar, Number2String(stats.Cmp));
        SetValue(ptiBar, Number2String(stats.Pti));
        SetValue(pthBar, Number2String(stats.Pth));
    }

    private void RefreshStats(ActorStats stats)
    {
        SetValue(hpBar, stats.Hp.ToString());
        SetValue(defBar, stats.Def.ToString());
        SetValue(agiBar, stats.Agi.ToString());
        SetValue(sanBar, stats.San.ToString());
        SetValue(cmpBar, stats.Cmp.ToString());
        SetValue(ptiBar, stats.Pti.ToString());
        SetValue(pthBar, stats.Pth.ToString());
    }

    private void RefreshSkillsAndStats()
    {
        RefreshStats(MergeUtil.CalculateStats(currentParts.
            Select(part => ActorBodyPartStats.FromCSV(part.Item.ID))));
        RefreshSkills(
            currentParts.SelectMany(part => part.Item.Active)
                .Distinct().Select(CSV.ActiveConf.Get).ToArray(),
            currentParts.SelectMany(part => part.Item.Passive)
                .Distinct().Select(CSV.PassiveConf.Get).ToArray());
    }

    private void ResetEditSlots() => partSlots.TraverseChildren(slot => SetSlot(slot, null));

    private void Reset()
    {
        PrepareData();

        ResetSkillPanel();
        RefreshStats(new ActorStats());

        ResetEditSlots();
    }

    private void OnConfirm()
    {
        var parts = currentParts.Select(part => part.Item);
        var name = inputActorName.Text;

        if (!MergeUtil.CanMerge(parts))
        {
            Log.W($"至少需要{CSV.ActorBodyPart.Component.Head.GetDescription()}、{CSV.ActorBodyPart.Component.Torso.GetDescription()}、{CSV.ActorBodyPart.Component.Head.GetDescription()}与一处{CSV.ActorBodyPart.Component.Limb.GetDescription()}");
            return;
        }
        if (name.Length == 0)
        {
            Log.W("名称至少需要一个字符");
            return;
        }
        if (UserData.Current.ActorBodies.Any(actor => actor.Name == name))
        {
            Log.W("存档中存在相同名称");
            return;
        }

        var ids = parts.Select(part => part.ID);

        (UserData.Current.ActorBodies ??= []).Add(MergeUtil.Merge(ids, name));
        if (UserData.Current.ActorBodies != null)
            foreach (var id in ids) UserData.Current.ActorBodyParts.Remove(id);

        Archive.Save();

        Init();
    }
}
