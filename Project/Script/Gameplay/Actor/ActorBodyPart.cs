using System;

namespace Franken;

/// <summary>
/// 角色的身体组件
/// </summary>
public class ActorBodyPart : ICustomSerializable
{
    public enum Quality
    {
        White,
        Blue,
        Purple,
    }

    [Flags]
    public enum Component
    {
        None = 0,
        Head = 1,
        Heart = 1 << 1,
        Torso = 1 << 2,
        Limb = 1 << 3,
        Upper = 1 << 4,
        Lower = 1 << 5,
        UpperLimb = Upper | Limb,
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