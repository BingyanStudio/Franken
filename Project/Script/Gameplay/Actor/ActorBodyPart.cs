using System;
using System.ComponentModel;

namespace Franken;

/// <summary>
/// 角色的身体组件
/// </summary>
public class ActorBodyPart : ICustomSerializable
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

    public Quality Qual { get; private set; }

    public Component Comp { get; private set; }

    public ActorBodyPartSkill Skill { get; private set; } = new();

    public ActorBodyPartStats Stats { get; private set; } = new();

    public void Upgrade()
    {
        if (Qual < Quality.Purple) Qual++;
        // TODO
    }

    public void Degrade()
    {
        if (Qual > Quality.White) Qual--;
        // TODO
    }

    public void Serialize(CustomSerializeData data)
    {
        data.Add(Name);
        EnumUtil.SerializeToNum(Qual, data);
        EnumUtil.SerializeToNum(Comp, data);
        Skill.Serialize(data);
        Stats.Serialize(data);
    }

    public void Deserialize(CustomSerializeData data)
    {
        Name = data.Get("default");
        Qual = EnumUtil.DeserializeFromNum<Quality>(data);
        Comp = EnumUtil.DeserializeFromNum<Component>(data);
        Skill.Deserialize(data);
        Stats.Deserialize(data);
    }
}