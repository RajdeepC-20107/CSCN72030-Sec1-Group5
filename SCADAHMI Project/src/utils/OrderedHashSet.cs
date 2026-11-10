using System;
using System.Collections;

namespace SCADAHMI_Project.src.utils;

public class OrderedHashSet<T> : IEnumerable<T>
{
    private readonly HashSet<T> set = new();
    private readonly List<T> list = new();

    public IReadOnlyList<T> Items => list;

    public int Count => set.Count;

    public bool Add(T item)
    {
        if (!set.Add(item))
            return false;

        list.Add(item);
        return true;
    }

    public bool Remove(T item)
    {
        if (!set.Remove(item))
            return false;

        list.Remove(item);
        return true;
    }

    public bool Contains(T item) => set.Contains(item);

    public int GetIndex(T item) => list.IndexOf(item);

    public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
