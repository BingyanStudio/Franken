using Godot;
using System;
using System.Collections.Generic;

namespace Franken;

public partial class TransitionExecutor : Node
{
    static TransitionExecutor() => (Engine.GetMainLoop() as SceneTree).Root.GetChild(0).AddChild(new TransitionExecutor());

    private readonly static List<Transition> anims = [];
    private readonly static Stack<Transition> disabled = new(), enabling = new();

    public static void Register(Transition anim) => enabling.Push(anim);
    public static void Unregister(Transition anim) => disabled.Push(anim);

    public override void _Process(double delta)
    {
        base._Process(delta);
        while (enabling.TryPop(out var e)) anims.Add(e);
        anims.ForEach(anim => anim.Tick(Convert.ToSingle(delta)));
        while (disabled.TryPop(out var d)) anims.Remove(d);
    }
}