using General;
using UnityEngine;

namespace Inventory
{
    public class Inventory : SingletonMonoBehavior<Inventory>
    {
        public bool[] isFull;
        public GameObject[] slots;
    }
}
