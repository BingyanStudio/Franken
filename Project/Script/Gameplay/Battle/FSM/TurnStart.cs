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
        CommandProcessor.ProcessCommand(new AssignHealPtCommand());

        await this.AwaitAeEnd();
    }

    public override BaseState NextState() => new SortAction();
}
