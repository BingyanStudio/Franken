using Franken;
using Godot;
using System;
using System.Collections.Generic;

namespace Franken;

[ObservableObject]
public partial class BattleManager : Node
{
    public static BattleManager Instance { get; private set; }
        
    //这里先直接给出，只需要纯粹的数值
    /// <summary>
    /// 行动顺序
    /// </summary>
    [ObservableProperty]
    private List<Unit> units;

    public override void _Ready()
    {
        Instance = this;
    }

    public override void _ExitTree()
    {
        Instance = null;
    }
}
