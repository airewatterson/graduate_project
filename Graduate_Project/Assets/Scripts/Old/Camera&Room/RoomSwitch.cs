using General;
using UnityEngine;

namespace Camera_Room
{
    public class RoomSwitch : SingletonMonoBehavior<RoomSwitch>
    {
        [SerializeField] private GameObject switchDestination;
        public GameObject door;
        public GameObject nextDoor;
        
        
        public void SwitchRoom()
        {
            
        }
    }
}
