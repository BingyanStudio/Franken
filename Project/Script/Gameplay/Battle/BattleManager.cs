using Franken;
using Franken.Utils;
using Godot;
using System;
using System.Collections.Generic;

namespace Franken;

[ObservableObject]
public partial class BattleManager : Node
{
    public static BattleManager Instance { get; private set; }
        
    /// <summary>
    /// 场上单元
    /// </summary>
    public ObservableCollection<Unit> Units { get; private set; }

    /// <summary>
    /// 场上格子
    /// </summary>
    public List<Grid> Grids { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public override void _ExitTree()
    {
        Instance = null;
    }
}
