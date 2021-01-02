using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableCollection
{
    class Wrap<T>
    {
        [SerializeField]
        T data;

        public Wrap(T data)
        {
            this.data = data;
        }

        public T Get { get => data; }
    }//이렇게 감싸주면 배열,리스트 등을 직렬화가능 
    public class CollectionList
    {
        [SerializeField]
        List<string> Data;
        //타입을 저장해서 쓸수 있지 않을까?

        public CollectionList()
        {
            Data = new List<string>();
        }
        public bool Set<T>(int index, T vaule)
        {
            Wrap<T> wrap = new Wrap<T>(vaule);
            string data = JsonUtility.ToJson(wrap);

            if(!string.IsNullOrEmpty(data))
            {
                if (index < Data.Count && index >= 0 && Data.Count != 0)
                {
                    Data[index] = data;
                    return true;
                }
                else
                {
                    return false;
                }
            }else
            {
                return false;
            }

        }
        public void Add<T>(T vaule)
        {
            Wrap<T> wrap = new Wrap<T>(vaule);
            string data = JsonUtility.ToJson(wrap);
            Data.Add(data);
        }
        public T Get<T>(int index)
        {
            if(index < Data.Count && index >= 0 && Data.Count != 0)
            {
                T vaule = JsonUtility.FromJson<Wrap<T>>(Data[index]).Get;
                return vaule;
            }else
            {
                return default;
            }
        }
    }
}
