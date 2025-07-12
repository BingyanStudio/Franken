namespace Franken;

/// <summary>
/// 身体部件技能tag
/// </summary>
public class ActorBodyPartSkill : ICustomSerializable
{
    /// <summary>
    /// 主动技能
    /// </summary>
    public string Active { get; set; }
    /// <summary>
    /// 被动技能
    /// </summary>
    public string Passive { get; set; }

    public void Serialize(CustomSerializeData data)
    {
        data.Add(Active);
        data.Add(Passive);
    }

    public void Deserialize(CustomSerializeData data)
    {
        Active = data.Get(string.Empty);
        Passive = data.Get(string.Empty);
    }
}