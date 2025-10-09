#if TOOLS
using Godot;

namespace Franken;

[Tool]
public partial class GMTools : EditorPlugin
{
	private EditorWindow window;

	private Foldout archive;

	private Foldout item;

	private LineInput inputItemId;
	private CustomButton btnAddItem;

	public override void _EnterTree()
	{
		window = AddonsUtil.CreateWidget<EditorWindow>();
		AddControlToDock(DockSlot.RightUl, window);

		window.Name = "GMTools";

		archive = window.AddContent(AddonsUtil.CreateWidget<Foldout>().SetTitle("存档"));

		item = archive.AddContent(AddonsUtil.CreateWidget<Foldout>().SetTitle("物品"));

		inputItemId = item.AddContent(AddonsUtil.CreateWidget<LineInput>().SetLabel("物品ID："));
		btnAddItem = item.AddContent(AddonsUtil.CreateWidget<CustomButton>());

		btnAddItem.Pressed += () =>
		{
		};
	}

	public override void _ExitTree()
	{
		RemoveControlFromDocks(window);
		window.Free();
	}
}
#endif
