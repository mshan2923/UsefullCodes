using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Map<T>
{
    [SerializeField]
    private List<T> Vaule;

    public Map(List<T> vaule = null)
    {
        Vaule = new List<T>();
        if (vaule != null)
        {
            Vaule = vaule;
        }
    }

    public List<T> Get()
    {
        return Vaule;
    }
    public T Get(int index)
    {
        if (Vaule.Count > index && index >= 0)
        {
            return Vaule[index];
        }
        else
        {
            return default;
        }
    }
    public bool Set(int index, T t = default)
    {
        if (Vaule.Count > index && index >= 0)
        {
            Vaule[index] = t;
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool Exist(T t)
    {
        return Vaule.Contains(t);
    }

    public void Add(T t = default)
    {
        Vaule.Add(t);
    }
    public void Remove(T t)
    {
        Vaule.Remove(t);
    }
    public void RemoveAt(int index)
    {
        Vaule.RemoveAt(index);
    }

    public int Length()
    {
        return Vaule.Count;
    }

    public static Map<T> operator +(Map<T> map, T t)
    {
        map.Add(t);
        return map;
    }
    public static Map<T> operator -(Map<T> map, T t)
    {
        map.Remove(t);
        return map;
    }
    public static Map<T> operator -(Map<T> map, int index)
    {
        map.RemoveAt(index);
        return map;
    }

}

[Serializable]
public class IntMap<T>
{
    [Serializable]
    public struct Vaule
    {
        public int key;
        public T vaule;
    }//int key , T vaule
    [SerializeField]
    private List<Vaule> Vaules;

    public IntMap(List<Vaule> vaules = null)
    {
        Vaules = new List<Vaule>();
        if (vaules != null)
        {
            Vaules = vaules;
        }
    }

    public List<Vaule> Get()
    {
        return Vaules;
    }
    public Vaule Get(int index)
    {
        if (Vaules.Count > index && index >= 0)
        {
            return Vaules[index];
        }
        else
        {
            return default;
        }
    }
    public Vaule GetKey(int Key)
    {
        if (Vaules.Exists(v => v.key == Key))
        {
            return Vaules.Find(v => v.key == Key);
        }
        else
        {
            return default;
        }
    }
    public T Find(int index)
    {
        if (Vaules.Count > index && index >= 0)
        {
            return Get(index).vaule;
        }
        else
        {
            return default;
        }
    }
    public T FindToKey(int Key)
    {
        return GetKey(Key).vaule;
    }
    public int FindIndex(int Key)
    {
        return Vaules.FindIndex(v => v.key == Key);
    }

    public bool Set(int index, T t = default)
    {
        if (Vaules.Count > index && index >= 0)
        {
            Vaules[index] = new Vaule { key = Vaules[index].key, vaule = t };
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool SetToKey(int Key, T t = default)
    {
        int temp = FindIndex(Key);
        if (temp >= 0)
        {
            Vaules[temp] = new Vaule { key = Key, vaule = t };
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Exist(Vaule vaule)
    {
        return Vaules.Contains(vaule);
    }
    public bool Exist(int Key)
    {
        return Vaules.Exists(v => v.key == Key);
    }

    public void Add(Vaule vaule = default)
    {
        Vaules.Add(vaule);
    }
    public void Add(int key, T t = default)
    {
        Vaule Temp = new Vaule
        {
            key = key,
            vaule = t
        };

        Vaules.Add(Temp);
    }
    public void Remove(Vaule vaule)
    {
        Vaules.Remove(vaule);
    }
    public void Remove(int Key)
    {
        RemoveAt(FindIndex(Key));
    }
    public void RemoveAt(int index)
    {
        Vaules.RemoveAt(index);
    }

    public static IntMap<T> operator +(IntMap<T> intMap, Vaule vaule)
    {
        intMap.Add(vaule);
        return intMap;
    }
    public static IntMap<T> operator +(IntMap<T> intMap, int key)
    {
        intMap.Add(key);
        return intMap;
    }
    public static IntMap<T> operator -(IntMap<T> intMap, Vaule vaule)
    {
        intMap.Remove(vaule);
        return intMap;
    }
    public static IntMap<T> operator -(IntMap<T> intMap, int key)
    {
        intMap.RemoveAt(key);
        return intMap;
    }

}
