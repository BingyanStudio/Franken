#if TOOLS
using Godot;
using Godotool;

namespace Franken;

[Tool]
public partial class GMTools : EditorPlugin
{
    private EditorWindow window;

    private Foldout archive;

    private Foldout management;

    private LineInput inputDataIdx;
    private CustomButton btnSetDataIdx;
    private CustomButton btnDelDataIdx;

    private Foldout item;

    private LineInput inputItemId;
    private LineInput inputItemCnt;
    private CustomButton btnAddItem;

    public override void _EnterTree()
    {
        window = AddonsUtil.CreateWidget<EditorWindow>().SetName("GMTools");
        AddControlToDock(DockSlot.RightUl, window);

        archive = window.AddContent(AddonsUtil.CreateWidget<Foldout>().SetTitle("存档"));

        management = archive.AddContent(AddonsUtil.CreateWidget<Foldout>().SetTitle("管理"));

        inputDataIdx = management.AddContent(AddonsUtil.CreateWidget<LineInput>().SetLabel("存档index：").SetInput($"{Archive.Current}"));
        btnSetDataIdx = management.AddContent(AddonsUtil.CreateWidget<CustomButton>().SetLabel("定位目标存档").SetPressed(OnSetDataIdx));
        btnDelDataIdx = management.AddContent(AddonsUtil.CreateWidget<CustomButton>().SetLabel("删除目标存档").SetPressed(OnDelDataIdx));

        item = archive.AddContent(AddonsUtil.CreateWidget<Foldout>().SetTitle("物品"));

        inputItemId = item.AddContent(AddonsUtil.CreateWidget<LineInput>().SetLabel("物品ID："));
        inputItemCnt = item.AddContent(AddonsUtil.CreateWidget<LineInput>().SetLabel("物品数量："));

        btnAddItem = item.AddContent(AddonsUtil.CreateWidget<CustomButton>().SetLabel("添加！").SetPressed(OnAddItem));
    }

    public override void _ExitTree()
    {
        RemoveControlFromDocks(window);
        window.Free();
    }

    private void OnDelDataIdx()
    {
        var dataIdx = inputDataIdx.Read().ToInt();

        if (dataIdx < 0 || dataIdx >= Archive.DATA_COUNT)
        {
            Log.E($"index不合法，应当为[0, {Archive.DATA_COUNT})范围内的整数！");
            return;
        }

        Archive.Delete(dataIdx);
        Log.I($"已删除{dataIdx}号存档！");
    }

    private void OnSetDataIdx()
    {
        var dataIdx = inputDataIdx.Read().ToInt();

        if (dataIdx < 0 || dataIdx >= Archive.DATA_COUNT)
        {
            Log.E($"index不合法，应当为[0, {Archive.DATA_COUNT})范围内的整数！");
            return;
        }

        Archive.Current = dataIdx;
        Log.I($"已定位至{dataIdx}号存档！");
    }

    private void OnAddItem()
    {
        var itemId = inputItemId.Read();
        var itemCnt = inputItemCnt.Read().ToInt();

        if (CSV.ActorBodyPart.Get(itemId) == null)
        {
            Log.E($"未找到{itemId}");
            return;
        }
        if (itemCnt <= 0)
        {
            Log.E("数量不合法");
            return;
        }

        Archive.Load();
        var data = Archive.Data[Archive.Current];
        var ids = data.ActorBodyParts ?? [];
        for (int i = 0; i < itemCnt; i++) ids.Add(itemId);
        data.ActorBodyParts = ids;
        Archive.Save();
        Log.I($"已向{Archive.Current}号存档中添加{itemCnt}个{itemId}！");
    }
}
#endif
