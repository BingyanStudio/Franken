using Godot;
using System;
using System.Threading.Tasks;

/// <summary>
/// 进入战斗的状态，负责分配行动点
/// </summary>
public partial class EnterBattle : BaseState
{

    public async override Task<BaseState> OnStateExecuteAsync()
    {
		await Task.Delay(1);
		return this;
    }
}
