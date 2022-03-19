using UnityEngine;
using System.Collections;

namespace General
{
    public abstract class SingletonMonoBehavior<T> : MonoBehaviour {
    
        private static T _instance;
    
        public virtual void Awake() {
            if (_instance == null || _instance.Equals(default(T)))
                _instance = (T)((object)this);
        }
    
        public static T Instance => _instance;
    }
}


