namespace Franken;

[ObservableObject]
public partial class ActorBodyPartSlot : BagItemBase<CSV.ActorBodyPart>
{
    public ActorBodyPartSlot(CSV.ActorBodyPart part) => Item = part;

    public override string Name => Item.Name;

    [ObservableProperty]
    private bool selected;
}