using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Franken.Utils;

/// <summary>
/// 对<see cref="ObservableCollection{T}"/>的拙劣模仿
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull where TValue : IComparable
{
    private readonly Dictionary<TKey, TValue> dictionary;

    #region Ctors
    public ObservableDictionary()
    {
        this.dictionary = new Dictionary<TKey, TValue>();
    }

    public ObservableDictionary(IEqualityComparer<TKey> comparer)
    {
        this.dictionary = new Dictionary<TKey, TValue>(comparer: comparer);
    }

    public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
    {
        this.dictionary = new Dictionary<TKey, TValue>(capacity, comparer: comparer);
    }

    public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
    {
        this.dictionary = new Dictionary<TKey, TValue>(collection: collection);
    }
    #endregion

    /// <summary>
    /// Invoked when <see cref="this[TKey]"/> modifies an existed pair to a new value
    /// </summary>
    public Action<TKey, TValue, TValue> OnDictionaryEdited;

    public TValue this[TKey key]
    {
        get
        {
            return dictionary[key];
        }
        set
        {
            if (dictionary.TryGetValue(key, out var oldValue))
            {
                //不相等才通知
                if (oldValue.CompareTo(value) != 0)
                {
                    dictionary[key] = value;
                    OnDictionaryEdited(key, oldValue, value);
                }
            }
            else
            {
                Add(key, value);
            }
        }
    }
    
    /// <summary>
    /// Invoked when <see cref="Add(KeyValuePair{TKey, TValue})"/> or <see cref="Add(TKey, TValue)"/> called
    /// </summary>
    public Action<TKey, TValue> OnDictionaryInserted;

    public void Add(TKey key, TValue value)
    {
        dictionary.Add(key, value);
        OnDictionaryInserted?.Invoke(key, value);
    }

    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

    /// <summary>
    /// Invoked when <see cref="Clear"/> called
    /// </summary>

    public Action OnDictionaryCleard;

    public void Clear()
    {
        dictionary.Clear();
        OnDictionaryCleard?.Invoke();
    }

    /// <summary>
    /// Invoked when <see cref="Remove(KeyValuePair{TKey, TValue})"/> or <see cref="Remove(TKey)"/> called
    /// </summary>
    public Action<TKey, TValue> OnDictionaryRemoved;

    public bool Remove(TKey key)
    {
        if (dictionary.Remove(key, out var value))
        {
            OnDictionaryRemoved?.Invoke(key, value);
            return true;
        }
        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (dictionary.TryGetValue(item.Key, out var value))
        {
            // 如果值相等才移除
            if (EqualityComparer<TValue>.Default.Equals(value, item.Value))
            {
                if (dictionary.Remove(item.Key, out var value2))
                {
                    OnDictionaryRemoved.Invoke(item.Key, value2);
                    return true;
                }
            }
        }
        return false;
    }

    #region Other Implementation
    public bool Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains(item);

    public bool ContainsKey(TKey key) => ((IDictionary<TKey, TValue>)dictionary).ContainsKey(key);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);

    public bool IsReadOnly => false;

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => this.dictionary.Keys;

    ICollection<TValue> IDictionary<TKey, TValue>.Values => this.dictionary.Values;

    public int Count => this.dictionary.Count;

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => dictionary.TryGetValue(key, out value);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var item in dictionary) yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEqualityComparer<TKey> Comparer => dictionary.Comparer;
    #endregion
}