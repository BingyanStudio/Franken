using Franken;
using Franken.Utils;
using Godot;
using System;
using System.Collections.Generic;

namespace Franken;

/// <summary>
/// 战斗数据存储器，只有纯粹的数据<br/>
/// 牢记此处所有数据都是通过深拷贝得来的，不要直接修改原数据<br/>
/// 牢记此处所有数据只有在战斗确定结束后才会写回，不要在半路复制回去<br/>
/// </summary>
public partial class BattleStats : Node
{
    #region 单例
    public override void _Ready() => Instance = this;

    public override void _ExitTree() => Instance = null;

    public static BattleStats Instance { get; private set; }
    #endregion

    #region 数据
    /// <summary>
    /// 场上单元
    /// </summary>
    public ObservableCollection<Unit> Orders { get; private set; }

    /// <summary>
    /// 场上格子
    /// </summary>
    public List<Grid> Grids { get; private set; }
    #endregion

    #region 原子操作
    public void AssignHealPt() => Orders.ForEach(u => u.ActorBody.Stats.Pt += u.ActorBody.Stats.Pth);
    
    public void AssignInitialPt() => Orders.ForEach(actor => actor.ActorBody.Stats.Pt = actor.ActorBody.Stats.Pti);

    public void AssignActionOrders() => Orders.Sort((l, r) => l.ActorBody.Stats.Ahead >= r.ActorBody.Stats.Ahead ? 1 : -1);
    #endregion
}
