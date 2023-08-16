using System.Collections;
using System.Collections.Generic;

namespace Utils;

public class CircularList<T> : IList<T>
{
    private readonly List<T> _list;

    public CircularList()
    {
        _list = new List<T>();
    }

    public IEnumerable<T> CycleFrom(T from, CycleDirection direction = CycleDirection.Forward)
    {
        var startIndex = _list.IndexOf(from);
        var step = direction == CycleDirection.Forward ? 1 : -1;

        for (int index = startIndex, count = 0; count < _list.Count; index += step, count++)
        {
            if (index < 0)
            {
                index += _list.Count; // Correct the negative index
            }

            index %= _list.Count; // Ensure index is within the bounds
            yield return _list[index];
        }
    }

    #region IList

    public int Count => _list.Count;
    public bool IsReadOnly => (_list as IList).IsReadOnly;

    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        _list.Add(item);
    }

    public void Clear()
    {
        _list.Clear();
    }

    public bool Contains(T item)
    {
        return _list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        return _list.Remove(item);
    }

    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        _list.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _list.RemoveAt(index);
    }

    public T this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    #endregion
}