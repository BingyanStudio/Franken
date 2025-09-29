using Godot;
using System;
using System.Threading.Tasks;


/// <summary>
/// 状态基类
/// </summary>
[GlobalClass]
public abstract partial class BaseState : Node
{
    /// <summary>
    /// 状态执行
    /// </summary>
    /// <returns></returns>
    public abstract Task<BaseState> OnStateExecuteAsync();
}
