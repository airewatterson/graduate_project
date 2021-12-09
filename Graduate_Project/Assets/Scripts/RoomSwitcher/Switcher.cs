using General;
using UnityEngine;

namespace RoomSwitcher
{
    public class Switcher : SingletonMonoBehavior<Switcher>
    {
        [SerializeField] private Camera cameraP1Pos;
        [SerializeField] private Camera cameraP2Pos;
        [SerializeField] private Transform originRoomCenter;
        [SerializeField] private Transform destinationRoomCenter;
        
        [SerializeField] private GameObject targetRoom;

        public float moveSp;

        private void OnTriggerEnter(Collider other)
        {

            if (other.gameObject.CompareTag("Player"))
            {
                RoomSwitchP1();
                Debug.Log("R1toR2_P1ok");
            }

            else if (other.gameObject.CompareTag("Player2"))
            {
                RoomSwitchP2();
                Debug.Log("R1toR2_P2ok");
            }
        }

        private void RoomSwitchP1()
        {
            cameraP1Pos.transform.position = Vector3.Lerp(originRoomCenter.position, destinationRoomCenter.position, Time.deltaTime * moveSp);
        }
        private void RoomSwitchP2()
        {
            cameraP2Pos.transform.position = Vector3.Lerp(originRoomCenter.position, destinationRoomCenter.position, Time.deltaTime * moveSp);
        }



    }
}
