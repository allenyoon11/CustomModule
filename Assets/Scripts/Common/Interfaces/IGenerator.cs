using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace neuroears.allen.utils
{
    public interface IGenerator<T>
    {
        public Dictionary<int, T> storeDict { get; }
        public T Generate(int type);
    }
    public interface IGenerator<T1, T2>
    {
        public Dictionary<int, T1> storeDict { get; }
        public T1 Generate(T2 model);
        public int GetHash(T2 model);
    }

}