using Inventory;
using Items.Func;
using Redo;
using Unity.Mathematics;
using UnityEngine;

namespace Items.LandMine
{
    public class LandMine : ItemInfo
    {
        [SerializeField] private GameObject flag;

        [SerializeField] internal GameObject mine;

        public void PutFlag()
        {
            var player1Pos = Player1.transform.position;
            var player2Pos = Player2.transform.position;
            if (GetComponentInParent<Slot>().transform.parent.CompareTag("Player"))
            {
                Instantiate(flag, new Vector3(player1Pos.x,player1Pos.y,player1Pos.z) , quaternion.identity);
            }
            else if (GetComponentInParent<Slot>().transform.parent.CompareTag("Player2"))
            {
                Instantiate(flag, new Vector3(player2Pos.x,player2Pos.y,player2Pos.z) , quaternion.identity);
            }

            GameObject o;
            (o = gameObject).SetActive(false);
            Destroy(o);
        }
    }
}
