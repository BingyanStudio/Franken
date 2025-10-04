using Godot;

namespace Franken;

/// <summary>
/// 角色的基本属性
/// </summary>
[ObservableObject]
public partial class ActorStats
{
    #region 上下限字段
    private static readonly string Path = "res://Assets/Config/General/Actor_Config.tres";

    static ActorStats()
    {
        var actorConfig = GD.Load<ActorConfig>(Path);

        if (actorConfig == null) LogTool.Error($"铸币吧没创建对{Path}");

        MinSan = actorConfig.MinSan;
        MaxSan = actorConfig.MaxSan;

        MinDef = actorConfig.MinDef;
        MaxDef = actorConfig.MaxDef;

        MinPt = actorConfig.MinPt;
        MaxPt = actorConfig.MaxPt;

        MinCmp = actorConfig.MinCmp;
        MaxCmp = actorConfig.MaxCmp;

        MinAgi = actorConfig.MinAgi;
        MaxAgi = actorConfig.MaxAgi;
    }

    public static readonly int MinSan;
    public static readonly int MaxSan;

    public static readonly int MinDef;
    public static readonly int MaxDef;

    public static readonly int MinPt;
    public static readonly int MaxPt;

    public static readonly int MinCmp;
    public static readonly int MaxCmp;

    public static readonly int MinAgi;
    public static readonly int MaxAgi;
    #endregion

    #region 属性
    /// <summary>
    /// 血量条上限，非状态值
    /// </summary>
    [ObservableProperty]
    private int maxHp;

    /// <summary>
    /// 血量条
    /// </summary>
    [ObservableProperty]
    private int hp;

    /// <summary>
    /// SAN值
    /// </summary>
    [ObservableClampProperty(minimum: "MinSan", maximum: "MaxSan")]
    private int san;

    /// <summary>
    /// 完整度
    /// </summary>
    [ObservableClampProperty(minimum: "MinCmp", maximum: "MaxCmp")]
    private int cmp;

    /// <summary>
    /// 行动点实际值，状态值
    /// </summary>
    [ObservableClampProperty(minimum: "MinPt", maximum: "MaxPt")]
    private int pt;

    /// <summary>
    /// 行动点回复值，非状态值
    /// </summary>
    [ObservableProperty]
    private int pth;

    /// <summary>
    /// 行动点初始值，非状态值
    /// </summary>
    [ObservableProperty]
    private int pti;

    /// <summary>
    /// 攻击力
    /// </summary>
    [ObservableProperty]
    private int atk;

    /// <summary>
    /// 防御力
    /// </summary>
    [ObservableClampProperty(minimum: "MinDef", maximum: "MaxDef")]
    private int def;

    /// <summary>
    /// 闪避值
    /// </summary>
    [ObservableClampProperty(minimum: "MinAgi", maximum: "MaxAgi")]
    private int agi;

    #endregion

    #region 二级属性
    /// <summary>
    /// SUCCESS常数
    /// </summary>
    public float SUCCESS => 1 + (Cmp / 100f);

    /// <summary>
    /// 闪避率
    /// </summary>
    public float Evade => SUCCESS * (Agi / 100f) * (1 - (Hp / (2f * MaxHp)));

    /// <summary>
    /// 暴击率
    /// </summary>
    public float Critical => SUCCESS * (San / 100f) * 0.5f;

    /// <summary>
    /// 先行值
    /// </summary>
    public float Ahead => Pt / (float)MaxPt * Agi;

    /// <summary>
    /// 爆抗值
    /// </summary>
    public float Resistance => Def * 0.35f / Agi;
    #endregion
}
