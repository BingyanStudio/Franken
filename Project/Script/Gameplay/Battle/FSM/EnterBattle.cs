using Godot;
using System;
using System.Threading.Tasks;

public partial class EnterBattle : BaseState
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public async override Task<BaseState> OnStateExecuteAsync()
    {
		await Task.Delay(1);
		return this;
    }
}
