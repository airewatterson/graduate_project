using Photon.Pun;
using UnityEngine;

namespace MultiPlayer
{
    public class SpawnPlayers : MonoBehaviour
    {
        public GameObject playerPrefab;
        [SerializeField] private Vector3 min;
        [SerializeField] private Vector3 max;
        
        // Start is called before the first frame update
        private void Start()
        {
            var randomPosition = new Vector3(Random.Range(min.x, max.x), 0, Random.Range(min.z, max.z));
            PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        }
    }
}
