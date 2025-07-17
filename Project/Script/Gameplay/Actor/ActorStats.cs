namespace Franken;

/// <summary>
/// 角色的基本属性
/// </summary>
public class ActorStats
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
}
