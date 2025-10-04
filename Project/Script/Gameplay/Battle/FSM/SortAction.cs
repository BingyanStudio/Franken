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
        CommandProcessor.ProcessCommand(new AssignActionOrdersCommand());

        // TODO: 根据BUFF决定是否能够行动
        //BattleStats.Instance.Units.ForEach(p => p.CanAct = true);
        
        await this.AwaitAeEnd();
    }

    public override BaseState NextState() => this;
}
