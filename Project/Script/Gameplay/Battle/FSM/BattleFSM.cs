using Godot;
using System;
using System.Threading.Tasks;

namespace Franken;

/// <summary>
/// 战斗流程状态机，用于自动切换流程
/// </summary>
[GlobalClass]
public partial class BattleFSM : Node
{
    [Export] private BaseState State { get; set; }
    
    public override void _Ready()
    {
        base._Ready();
        _ = FSMLoop();
    }

    /// <summary>
    /// 启动之后就开始循环
    /// </summary>
    private async Task FSMLoop()
    {
        await State.ExecuteAsync();
        State = State.NextState();
    }
}
