#if TOOLS
using Godot;
using Godotool;
using System.Linq;

namespace Franken;

[Tool]
public partial class GMTools : EditorPlugin
{
    private EditorWindow window;

    private Foldout menuArchive;

    private Foldout menuManagement;

    private LinePopup popupDataIdx;
    private CustomButton btnDelDataIdx;

    private Foldout menuItem;

    private LinePopup popupItemId;
    private LineInput inputItemCnt;
    private CustomButton btnAddItem;

    public override void _EnterTree()
    {
        window = AddonsUtil.CreateWidget<EditorWindow>().SetName("GMTools");
        AddControlToDock(DockSlot.RightUl, window);

        menuArchive = window.AddContent(AddonsUtil.CreateWidget<Foldout>().SetTitle("存档"));

        menuManagement = menuArchive.AddContent(AddonsUtil.CreateWidget<Foldout>().SetTitle("管理"));

        popupDataIdx = menuManagement.AddContent(AddonsUtil.CreateWidget<LinePopup>().SetLabel("存档index：").OnSelected(idx => Archive.Current = idx.ToInt()).AddData(IEnumerableUtil.Range(0, Archive.DATA_COUNT).Select(idx => idx.ToString())).SetDefault(0));
        btnDelDataIdx = menuManagement.AddContent(AddonsUtil.CreateWidget<CustomButton>().SetLabel("删除目标存档").OnPressed(OnDelDataIdx));

        menuItem = menuArchive.AddContent(AddonsUtil.CreateWidget<Foldout>().SetTitle("物品"));

        popupItemId = menuItem.AddContent(AddonsUtil.CreateWidget<LinePopup>().SetLabel("物品ID：").AddData(CSV.ActorBodyPart.Data.Select(data => data.ID)).SetDefault(0));
        inputItemCnt = menuItem.AddContent(AddonsUtil.CreateWidget<LineInput>().SetLabel("物品数量："));

        btnAddItem = menuItem.AddContent(AddonsUtil.CreateWidget<CustomButton>().SetLabel("添加！").OnPressed(OnAddItem));
    }

    public override void _ExitTree()
    {
        RemoveControlFromDocks(window);
        window.Free();
    }

    private void OnDelDataIdx()
    {
        var dataIdx = popupDataIdx.Read().ToInt();

        if (dataIdx < 0 || dataIdx >= Archive.DATA_COUNT)
        {
            Log.E($"index不合法，应当为[0, {Archive.DATA_COUNT})范围内的整数！");
            return;
        }

        Archive.Delete(dataIdx);
        Log.I($"已删除{dataIdx}号存档！");
    }

    private void OnAddItem()
    {
        var itemId = popupItemId.Read();
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
