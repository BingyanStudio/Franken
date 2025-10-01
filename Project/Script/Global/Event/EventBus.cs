using Godot;
using System;
using System.Threading.Tasks;

namespace Franken;

/// <summary>
/// 事件总线，一定要用Autoload加载
/// </summary>
[GlobalClass, EventBus]
public partial class EventBus : Node
{
    /// <summary>
    /// 动效结束信号，一般在<see cref="BaseState.ExecuteAsync"/>中使用以同步FSM
    /// </summary>
    [Signal] public delegate void AeEndEventHandler();
}
