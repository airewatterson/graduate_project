using System.Security.AccessControl;
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

        //移動玩家位置
        private GameObject _player;
        [SerializeField] private GameObject targetRoom;
        
        //移動鏡頭速度
        public float moveSp;
        
        //指定下間房間位置
        [Header("指定房間方位，一間房間僅允許勾選一項")]
        [SerializeField] private bool up;
        [SerializeField] private bool down;
        [SerializeField] private bool left;
        [SerializeField] private bool right;

        private void OnTriggerEnter(Collider other)
        {

            if (other.gameObject.CompareTag("Player"))
            {
                _player = GameManager.Instance.player1;
                RoomSwitchP1();
                PlayerSwitch();
                Debug.Log("Room1 to Room2 : Player1");
            }

            else if (other.gameObject.CompareTag("Player2"))
            {
                _player = GameManager.Instance.player2;
                RoomSwitchP2();
                PlayerSwitch();
                Debug.Log("Room1 to Room2 : Player2");
            }
            else if(other.gameObject.CompareTag("Enemy"))
            {
                _player = GameObject.FindWithTag("Enemy");
                PlayerSwitch();
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

        private void PlayerSwitch()
        {
            var positionPlayer = _player.transform.position;
            var position = targetRoom.transform.position;
            if (right)
            {
                
                _player.transform.position = new Vector3(position.x+1.5f,positionPlayer.y,positionPlayer.z);
            }
            else if (left)  
            {
                _player.transform.position = new Vector3(position.x-1.5f,positionPlayer.y,positionPlayer.z);
            }
            else if (down)  
            {
                _player.transform.position = new Vector3(positionPlayer.x,positionPlayer.y,position.z-1.5f);
            }
            else if (up)  
            {
                _player.transform.position = new Vector3(positionPlayer.x,positionPlayer.y,position.z+1.5f);
            }
            
        }

    }
}
