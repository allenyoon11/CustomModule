using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace neuroears.allen.utils
{
    public class GameObjectPool<T> where T : MonoBehaviour
    {
        public List<T> Pool { get; }

        private Transform container;
        private T prefab;
        private int maxSize;
        private int defaultSize;
        private int storeCount; // 0 <= storeCount <= maxSize;
        private int activedCount; // Pool.Where(item => item.gameObject.activeSelf).Count();
        public GameObjectPool(
            Transform container,
            T prefab,
            int maxSize,
            int defaultSize = 0
            )
        {
            this.container = container;
            this.prefab = prefab;
            this.maxSize = Mathf.Max(0, maxSize);
            this.defaultSize = Mathf.Max(0, defaultSize);
            this.storeCount = 0;
            this.activedCount = 0;
            this.Pool = new List<T>();

            if (defaultSize > 0)
            {
                for (int i = 0; i < defaultSize; i++)
                {
                    T item = Get();
                }
            }
        }
        public T Get()
        {
            if (activedCount < storeCount)
            {
                var storedItem = Pool.Where(item => !item.gameObject.activeSelf).FirstOrDefault();
                GetItem(storedItem);
                return storedItem;
            }

            var newItem = CreateItem();
            GetItem(newItem);
            return newItem;
        }
        public void Release(T obj)
        {
            if (activedCount == 0) return;
            if (Pool.Count > maxSize)
            {
                DestroyItem(obj);
            }
            else
            {
                ReleaseItem(obj);
            }
        }
        public void Release()
        {
            if (activedCount == 0) return;
            if (Pool.Count > maxSize)
            {
                DestroyItem(Pool.Last());
            }
            else
            {
                ReleaseItem(Pool.Where(item => item.gameObject.activeSelf).Last());
            }
        }
        public void ReleaseAll()
        {
            if (Pool.Count == 0) return;
            while (true)
            {
                if (Pool.Count <= maxSize)
                {
                    foreach (T item in Pool)
                    {
                        ReleaseItem(item);
                    }
                    break;
                }
                if (Pool.Count > maxSize)
                {
                    DestroyItem(Pool.Last());
                }
            }
        }
        public void DestroyAll()
        {
            foreach (T item in Pool)
            {
                DestroyItem(item);
            }
            Pool.Clear();
            activedCount = 0;
            storeCount = 0;
        }

        #region private
        private T CreateItem()
        {
            T item = Object.Instantiate<T>(this.prefab, container, false);
            storeCount = Mathf.Clamp(storeCount + 1, 0, maxSize);
            return item;
        }
        private void GetItem(T obj)
        {
            if (!Pool.Contains(obj))
                Pool.Add(obj);
            obj.gameObject.SetActive(true);
            activedCount += 1;
        }
        private void ReleaseItem(T obj)
        {
            //Debug.Log($"{obj} {obj.gameObject}");
            if(obj.gameObject.activeSelf)
                obj.gameObject.SetActive(false);
            activedCount = Mathf.Max(0, activedCount - 1);
        }
        private void DestroyItem(T obj)
        {
            if(obj.gameObject.activeSelf)
                Object.Destroy(obj.gameObject);
            Pool.Remove(obj);
            activedCount = Mathf.Max(0, activedCount - 1);
            if (Pool.Count < maxSize)
            {
                storeCount = Mathf.Clamp(storeCount - 1, 0, maxSize);
            }
        }
        #endregion
    }

}
