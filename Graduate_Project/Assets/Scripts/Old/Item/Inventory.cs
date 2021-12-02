using System.ComponentModel;
using General;
using UnityEngine;

namespace Item
{
    public class Inventory : SingletonMonoBehavior<Inventory>
    {
        public bool[] isFull;
        public GameObject[] slots;
        
    }
}
