namespace Franken;

public abstract class BagItemBase
{
    public abstract string Name { get; }
}

[ObservableObject]
public abstract partial class BagItemBase<T> : BagItemBase where T : class
{
    [ObservableProperty]
    private T item;
}
