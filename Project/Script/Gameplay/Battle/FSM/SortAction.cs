using Godot;
using System;
using System.Threading.Tasks;

namespace Franken;
/// <summary>
/// 决定行动顺序
/// </summary>
public partial class SortAction : BaseState
{
    public async override Task ExecuteAsync()
    {
        //根据先行值决定行动顺序
        BattleManager.Instance.Units.Sort((l, r) => l.ActorBody.Stats.Ahead >= r.ActorBody.Stats.Ahead ? 1 : -1);

        // TODO: 根据BUFF决定是否能够行动
        BattleManager.Instance.Units.ForEach(p => p.CanAct = true);

        await this.AwaitAeEnd();
    }

    public override BaseState NextState() => this;
}
