using System.Collections.Generic;


/// <summary>
/// 5/17/2024-LYI
/// Dictionary의 키와 밸류가 1:1로 대응이 되는경우 양방향 탐색이 가능하게 해주는 클래스
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
public class BiDirectionalDictionary<T1, T2>
{
    public int Count;
    private Dictionary<T1, T2> forwardMap = new Dictionary<T1, T2>();
    private Dictionary<T2, T1> reverseMap = new Dictionary<T2, T1>();

    public void Add(T1 key, T2 value)
    {
        forwardMap[key] = value;
        reverseMap[value] = key;

        Count = forwardMap.Count;
    }

    public bool ContainsKey(T1 key)
    {
        return forwardMap.ContainsKey(key);
    }
    public bool ContainsValue(T2 value)
    {
        return reverseMap.ContainsKey(value);
    }


    public bool TryGetByKey(T1 key, out T2 value)
    {
        return forwardMap.TryGetValue(key, out value);
    }

    public bool TryGetByValue(T2 value, out T1 key)
    {
        return reverseMap.TryGetValue(value, out key);
    }

    public void RemoveByKey(T1 key)
    {
        if (forwardMap.TryGetValue(key, out T2 value))
        {
            forwardMap.Remove(key);
            reverseMap.Remove(value);
            Count = forwardMap.Count;
        }
    }

    public void RemoveByValue(T2 value)
    {
        if (reverseMap.TryGetValue(value, out T1 key))
        {
            reverseMap.Remove(value);
            forwardMap.Remove(key);
            Count = forwardMap.Count;
        }
    }

    // Indexer for forwardMap
    public T2 this[T1 key]
    {
        get => forwardMap[key];
        set
        {
            if (forwardMap.ContainsKey(key))
            {
                reverseMap.Remove(forwardMap[key]);
            }
            forwardMap[key] = value;
            reverseMap[value] = key;
        }
    }

    // Indexer for reverseMap
    public T1 this[T2 valueKey]
    {
        get => reverseMap[valueKey];
        set
        {
            if (reverseMap.ContainsKey(valueKey))
            {
                forwardMap.Remove(reverseMap[valueKey]);
            }
            reverseMap[valueKey] = value;
            forwardMap[value] = valueKey;
        }
    }

}