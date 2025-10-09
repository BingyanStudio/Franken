using Godot;

namespace Franken;

public interface IManager
{
    void Init();
}

public partial class GameManager : Node
{
    private const string MANAGER_PATH = "res://Assets/Prefab/Manager/";

    [Export]
    private string[] managers;

    public override void _EnterTree()
    {
        base._EnterTree();

        InitManagers();
    }

    private void InitManagers() => managers.ForEach(name =>
    {
        var node = ResourceLoader.Load<PackedScene>($"{MANAGER_PATH}{name}.tscn")?.Instantiate();
        if (node is IManager manager)
        {
            AddChild(node);
            manager.Init();
        }
    });
}
