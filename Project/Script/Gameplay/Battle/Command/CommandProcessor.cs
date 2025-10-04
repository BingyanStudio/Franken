using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Franken;

/// <summary>
/// 基本指令接口，用于对数据产生原子性的影响，且只允许通过ICommand修改
/// </summary>
public interface ICommand
{
    void Execute(BattleStats target);
}

/// <summary>
/// 指令处理器
/// </summary>
public partial class CommandProcessor : Node
{
    public override void _Ready() => Instance = this;

    public override void _ExitTree() => Instance = null;

    public static CommandProcessor Instance { get; private set; }

    public static void ProcessCommand(ICommand command)
    {
        command.Execute(BattleStats.Instance);

        //TODO:输出日志方便找Bug,这也是为什么要走中间商亏GC
    }
}



