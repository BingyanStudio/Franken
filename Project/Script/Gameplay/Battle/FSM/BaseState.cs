using Godot;
using System;
using System.Threading.Tasks;

namespace Franken;
/// <summary>
/// 状态基类
/// </summary>
[GlobalClass]
public abstract partial class BaseState : Node
{
    /// <summary>
    /// 状态切换
    /// </summary>
    /// <returns></returns>
    public abstract BaseState NextState();

    /// <summary>
    /// 状态执行
    /// </summary>
    /// <returns></returns>
    public abstract Task ExecuteAsync();
}
