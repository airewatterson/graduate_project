using General;
using UnityEngine;

namespace Redo.Inventory
{
    public class Inventory : SingletonMonoBehavior<Inventory>
    {
        public bool[] isFull;
        public GameObject[] slots;
    }
}
