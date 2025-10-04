using Franken;
using Godot;
using System;

namespace Franken;

/// <summary>
/// 战斗单元
/// </summary>
[ObservableObject]
public partial class Unit
{
    /// <summary>
    /// 阵营
    /// </summary>
    public enum Faction
    {
        Ally = 0,
        Enemy = 1
    };

    [ObservableProperty]
    private Faction team;

    [ObservableProperty]
    private ActorBody actorBody;

    [ObservableProperty]
    private bool canAct;

    //public BuffDict Buffs { get; private set; }
}
