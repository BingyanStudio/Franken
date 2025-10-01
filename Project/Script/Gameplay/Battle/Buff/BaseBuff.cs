using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Franken;
/// <summary>
/// 抽象Buff基类
/// </summary>
[ObservableObject]
public abstract class BaseBuff
{
    [Flags]
    public enum TriggerPhase
    {
        None = 0,
        OnTurnBegin = 1 << 0,
        OnTrunEnd = 1 << 1,
    }

    /// <summary>
    /// 执行结算
    /// </summary>
    /// <returns></returns>
    public abstract Task ExecuteAsync();

    /// <summary>
    /// 触发时机
    /// </summary>
    public abstract TriggerPhase Phase { get; set; }

    /// <summary>
    /// 触发条件
    /// </summary>
    public abstract Predicate<bool> Condition { get; set; }

    [ObservableProperty]
    private int level;

}
