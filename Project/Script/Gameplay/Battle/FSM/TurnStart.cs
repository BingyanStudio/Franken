using Godot;
using System;
using System.Threading.Tasks;

namespace Franken;

public partial class TurnStart : BaseState
{
    public async override Task ExecuteAsync()
    {
        //TODO:结算Buff

        //TODO:添加行动点
        BattleManager.Instance.Units.ForEach(u => u.ActorBody.Stats.Pt += u.ActorBody.Stats.Pth);

        await this.AwaitAeEnd();
    }

    public override BaseState NextState() => new SortAction();
}
