using System.Collections.Generic;
using UnityEngine;

namespace AirplaneView
{
    public class AirportPool
    {
        public static GameObject Prefab;
        public static Transform Parent;
        private static Queue<GameObject> _pool;
        public static GameObject Get()
        {
            if (_pool == null)
            {
                Init();
            }

            if (_pool.Count <= 0)
            {
                var go= Object.Instantiate(Prefab, Parent);
                go.SetActive(true);
                
                return go;
            }

            return _pool.Dequeue();
        }
        
        

        public static void Back(GameObject go)
        {
            go.SetActive(false);
            _pool.Enqueue(go);
        }

        private static void Init()
        {
            _pool = new Queue<GameObject>(128);
        }
    }
}