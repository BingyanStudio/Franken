using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Franken.Utils;

/// <summary>
/// 对<see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>的拙劣模仿，简化版
/// </summary>
public class ObservableCollection<T> : Collection<T>
{
    public ObservableCollection(){ }

    public ObservableCollection(IEnumerable<T> collection) : base(new List<T>(collection ?? throw new ArgumentNullException(nameof(collection)))) { }

    public ObservableCollection(List<T> list) : base(new List<T>(list ?? throw new ArgumentNullException(nameof(list)))) { }

    /// <summary>
    /// Invoked when <see cref="Collection{T}.Clear"/> called
    /// </summary>
    public Action OnCollectionCleared;
    /// <summary>
    /// Called by <see cref="Collection{T}.Clear"/>
    /// </summary>
    protected override void ClearItems()
    {
        base.ClearItems();
        OnCollectionCleared?.Invoke();
    }

    /// <summary>
    /// Invoked when <see cref="Collection{T}.Remove(T)"/> or <see cref="Collection{T}.RemoveAt(int)"/> called
    /// </summary>

    public Action<T> OnCollectionRemoved;
    /// <summary>
    /// Called by <see cref="Collection{T}.Remove(T)"/> and <see cref="Collection{T}.RemoveAt(int)"/>
    /// </summary>
    protected override void RemoveItem(int index)
    {
        T removedItem = this[index];
        base.RemoveItem(index);
        OnCollectionRemoved?.Invoke(removedItem);
    }

    /// <summary>
    /// Invoked when <see cref="Collection{T}.Add(T)"/> or <see cref="Collection{T}.Insert(int, T)"/> called
    /// </summary>
    public Action<int, T> OnCollectionInserted;
    /// <summary>
    /// Called by <see cref="Collection{T}.Add(T)"/> and <see cref="Collection{T}.Insert(int, T)"/>
    /// </summary>
    protected override void InsertItem(int index, T item)
    {
        base.InsertItem(index, item);
        OnCollectionInserted?.Invoke(index, item);
    }

    /// <summary>
    /// Invoked when <see cref="Sort()"/> or <see cref="Sort(Comparison{T})"/> or <see cref="Sort(IComparer{T})"/> called
    /// </summary>
    public Action OnCollectionSorted;
    
    public void Sort() => Sort(Comparer<T>.Default);

    public void Sort(IComparer<T> comparer) => Sort(comparer.Compare);
    
    public void Sort(Comparison<T> comparison)
    {
        if (comparison == null) throw new ArgumentNullException(nameof(comparison));

        var sortedList = new List<T>(this.Items);
        if (sortedList.Count < 2) return;
        sortedList.Sort(comparison);

        for (int i = 0; i < sortedList.Count; i++) this.Items[i] = sortedList[i];
        
        OnCollectionSorted?.Invoke();
    }
}
