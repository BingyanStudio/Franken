using System;
using System.Collections.Generic;

namespace Franken;

public partial class TransitionManager : Singleton<TransitionManager>, IManager
{
    public void Init()
    {
        anims = [];

        disabled = [];
        enabling = [];
    }

    private List<Transition> anims;
    private Stack<Transition> disabled, enabling;

    public void Register(Transition anim) => enabling.Push(anim);
    public void Unregister(Transition anim) => disabled.Push(anim);

    public override void _Process(double delta)
    {
        base._Process(delta);
        while (enabling.TryPop(out var e)) anims.Add(e);
        anims.ForEach(anim => anim.Tick(Convert.ToSingle(delta)));
        while (disabled.TryPop(out var d)) anims.Remove(d);
    }
}