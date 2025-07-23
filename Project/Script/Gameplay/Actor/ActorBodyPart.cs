using System;
using System.ComponentModel;

namespace Franken;

/// <summary>
/// 角色的身体组件
/// </summary>
public class ActorBodyPart
{
    public enum Quality
    {
        [Description("白")]
        White,
        [Description("蓝")]
        Blue,
        [Description("紫")]
        Purple,
    }

    [Flags]
    public enum Component
    {
        None = 0,
        [Description("头部")]
        Head = 1,
        [Description("心脏")]
        Heart = 1 << 1,
        [Description("躯干")]
        Torso = 1 << 2,
        Limb = 1 << 3,
        Upper = 1 << 4,
        Lower = 1 << 5,
        [Description("上肢")]
        UpperLimb = Upper | Limb,
        [Description("下肢")]
        LowerLimb = Lower | UpperLimb,
    }

    public string Name { get; private set; }

    public Component Comp => CSV.ActorBodyPart.Get(Name).Comp;

    public ActorBodyPartStats Stats => ActorBodyPartStats.FromCSV(Name);
}