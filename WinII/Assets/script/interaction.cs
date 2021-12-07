using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class interaction : MonoBehaviour
{

    public Animator UP;
    public winCount winCount;

    public Text GetOrMissText;

    private void Awake()
    {

    }
    private void Start()
    {
        
    }

    // is Kinematic要勾   判斷P1 P2的互動動畫測試 OK  判斷 P1 P2 拿到勝利道具測試 OK
    // onTrigger在判斷玩家在裡面時會有延遲問題 是可以調整的不用程式
    // 程式目前 如果 P1 P2 在同個物件尋找是可以重複找到key 的  (如果要物件 " 被找到 " 後就不能找的話之後再調整)
    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag =="P1")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {    
                UP.SetBool("up", true);
                if (winCount.keyP1Count < 4) { 
                int RollChance = Random.Range(1, 3); // 50 %  
                
                if (RollChance == 2) {
                    winCount.keyP1Count++;
                    Debug.Log(winCount.keyP1Count);
                        GetOrMissText.text = "Get";
                        Destroy(this.GetOrMissText, 3);

                } else  {
                        Debug.Log("failed");
                        GetOrMissText.text = "Miss";
                        Destroy(this.GetOrMissText, 3);
                    }

                }  else  {
                    Debug.Log("searched");
                }

            }
            
        }
        else if (col.gameObject.tag =="P2")
        {
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                UP.SetBool("up", true);
                if (winCount.keyP2Count < 4)
                {
                    int RollChance = Random.Range(1, 3); // 50 %  

                    if (RollChance == 2)
                    {
                        winCount.keyP2Count++;
                        GetOrMissText.text = "Get";
                        Destroy(this.GetOrMissText, 3);

                        Debug.Log(winCount.keyP2Count);
                    }
                    else
                    {
                        GetOrMissText.text = "Miss";
                        Debug.Log("failed");
                        Destroy(this.GetOrMissText, 3);
                    }

                }
                else
                {
                    Debug.Log("searched");
                }
            }
          
        }
    }

    // P1 P2 離開Trigger範圍後判斷動畫回到IDLE
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "P1")
        {                   
                UP.SetBool("up", false);
                Debug.Log("OUT1");          
        }
        else if (col.gameObject.tag == "P2")
        {
            UP.SetBool("up", false);
            Debug.Log("OUT2");

        }

    }

}
