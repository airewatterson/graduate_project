using Input;
using UnityEngine;

namespace TRAP
{
    
    //看的懂。
    public class Trap : GameManager
    {
        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Player1.Instance.movement-= 3f;
                print(Player1.Instance.movement);
            }
            else if (col.gameObject.CompareTag("Player2"))
            {
                Player2.Instance.movement-= 3f;
                print(Player2.Instance.movement);
            }
       
        }
        private void OnTriggerExit(Collider col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Player1.Instance.movement += 3f;
                print(Player1.Instance.movement);
            }
            else if (col.gameObject.CompareTag("Player2"))
            {
                Player2.Instance.movement += 3f;
                print(Player2.Instance.movement);
            }
        }






    }
}
