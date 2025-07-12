namespace Franken;

/// <summary>
/// 单向链表，
/// </summary>
/// <typeparam name="T"></typeparam>
public class LinkList<T>
{
    private class Node
    {
        public T Value { get; set; }
        public Node Next { get; set; }

        public Node() { }
        public Node(T value) => Value = value;
        public Node(ref T value) => Value = value;
    }

    private Node head, tail;

    public LinkList() => head = tail = new();

    /// <summary>在链表尾部添加一个元素</summary>
    public void Add(T value) => tail = tail.Next = new(value);
    /// <summary>在链表尾部添加一个元素</summary>
    /// <param name="value">太大不方便复制的可以传引用</param>
    public void Add(ref T value) => tail = tail.Next = new(ref value);

    public bool Empty => head == tail;

    /// <summary>获取当前结点的值并移动到下一个结点</summary>
    public T Next => (head = head.Next).Value;

    /// <summary>获取当前结点的值</summary>
    public T Current => head.Next.Value;

    /// <returns>链表当前有无结点</returns>
    public bool TryNext(out T value) => TryNext(out value, default);
    /// <param name="def">链表为空则返回默认值</param>
    /// <returns>链表当前有无结点</returns>
    public bool TryNext(out T value, T def)
    {
        if (Empty)
        {
            value = def;
            return false;
        }
        value = Next;
        return true;
    }
}