namespace Franken;

/// <summary>
/// 角色的基本属性
/// </summary>
public class ActorStats : ICustomSerializable
{
    /// <summary>
    /// 血量条
    /// </summary>
    public int Hp { get; set; }
    /// <summary>
    /// SAN值
    /// </summary>
    public int San { get; set; }
    /// <summary>
    /// 能量条
    /// </summary>
    public int Mp { get; set; }
    /// <summary>
    /// 行动点
    /// </summary>
    public int Pt { get; set; }
    /// <summary>
    /// 攻击力
    /// </summary>
    public int Atk { get; set; }
    /// <summary>
    /// 防御力
    /// </summary>
    public int Def { get; set; }
    /// <summary>
    /// 闪避值
    /// </summary>
    public int Agi { get; set; }

    public void Serialize(CustomSerializeData data)
    {
        data.Add(Hp.ToString());
        data.Add(San.ToString());
        data.Add(Mp.ToString());
        data.Add(Pt.ToString());
        data.Add(Atk.ToString());
        data.Add(Def.ToString());
        data.Add(Agi.ToString());
    }

    public void Deserialize(CustomSerializeData data)
    {
        Hp = data.Get(0);
        San = data.Get(0);
        Mp = data.Get(0);
        Pt = data.Get(0);
        Atk = data.Get(0);
        Def = data.Get(0);
        Agi = data.Get(0);
    }
}
