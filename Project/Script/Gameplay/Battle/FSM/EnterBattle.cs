using Franken;
using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Franken;
/// <summary>
/// 进入战斗的状态，负责分配行动点
/// </summary>
public partial class EnterBattle : BaseState
{

    public async override Task ExecuteAsync()
    {
        //分配初始行动点
        BattleManager.Instance.Units.ForEach(actor => actor.ActorBody.Stats.Pt = actor.ActorBody.Stats.Pti);
        
        await this.AwaitAeEnd();
    }

    public override BaseState NextState() => new SortAction();

}
