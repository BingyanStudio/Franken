using Franken;
using Godot;
using System;
using System.Collections.Generic;

public partial class BattleManager : Node
{
    public BattleManager Instance { get; private set; }

    //这里先直接给出，只需要纯纯的数值
    public List<ActorBody> Allies { get; private set; }
    public List<ActorBody> Enemies { get; private set; }


    public override void _Ready()
    {
        Instance = this;
    }

    public override void _ExitTree()
    {
        Instance = null;
    }
}
