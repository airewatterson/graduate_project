using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiPlayer
{
    public class CreateAndJoiningRooms : MonoBehaviourPunCallbacks
    {
        [SerializeField] private InputField nameInput;

        public void CreateRoom()
        {
            PhotonNetwork.CreateRoom(nameInput.text);
        }

        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom(nameInput.text);
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel(2);
        }

        public void ExitRoom()
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel(1);
        }
    }
}
