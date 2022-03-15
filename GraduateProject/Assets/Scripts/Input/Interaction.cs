using UnityEngine;
using UnityEngine.Serialization;

namespace script
{
    public class Interaction : MonoBehaviour
    {

        [FormerlySerializedAs("UP")] public Animator up;
        //public winCount winCount;

        private static readonly int Up = Animator.StringToHash("up");


        // is Kinematic要勾   判斷P1 P2的互動動畫測試 OK  判斷 P1 P2 拿到勝利道具測試 OK
        // onTrigger在判斷玩家在裡面時會有延遲問題 是可以調整的不用程式
        // 程式目前 如果 P1 P2 在同個物件尋找是可以重複找到USB 的  (如果要物件 " 被找到 " 後就不能找的話之後再調整)
        private void OnTriggerStay(Collider col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                if (!UnityEngine.Input.GetKeyDown(KeyCode.E)) return;
                up.SetBool(Up, true);
                if (GameManager.Instance.player1CollectItem < 4) { 
                    var rollChance = Random.Range(1, 3); // 50 %  之後再別的物件改機率就好
                
                    if (rollChance == 2) {
                        GameManager.Instance.player1CollectItem++;
                        Debug.Log(GameManager.Instance.player1CollectItem);
                    } else  {
                        Debug.Log("failed");
                    }

                }  else  {
                    Debug.Log("searched");
                }


            }
            else if (col.gameObject.CompareTag("Player2"))
            {
                if (!UnityEngine.Input.GetKeyDown(KeyCode.RightShift)) return;
                up.SetBool(Up, true);
                if (GameManager.Instance.player2CollectItem < 4)
                {
                    var rollChance = Random.Range(1, 3); // 50 %  之後再別的物件改機率就好

                    if (rollChance == 2)
                    {
                        GameManager.Instance.player2CollectItem++;  
                        Debug.Log(GameManager.Instance.player2CollectItem);
                    }
                    else
                    {
                        Debug.Log("failed");
                    }

                }
                else
                {
                    Debug.Log("searched");
                }

            }
        }

        // P1 P2 離開Trigger範圍後判斷動畫回到IDLE
        private void OnTriggerExit(Collider col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
           
            
                up.SetBool(Up, false);
                Debug.Log("OUT1");
            

            }
            else if (col.gameObject.CompareTag("Player2"))
            {
                up.SetBool(Up, false);
                Debug.Log("OUT2");


            }



        }




    }
}
