using Godot;
using System;

/// <summary>
/// 角色相关的配置
/// </summary>
[GlobalClass]
public partial class ActorConfig : Resource
{
    [Export] public int MinSan;
    [Export] public int MaxSan;

    [Export] public int MinCmp;
    [Export] public int MaxCmp;

    [Export] public int MinDef;
    [Export] public int MaxDef;

    [Export] public int MinPt;
    [Export] public int MaxPt;

    [Export] public int MinAgi;
    [Export] public int MaxAgi;
}
