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

    // is Kinematic�n��   �P�_P1 P2�����ʰʵe���� OK  �P�_ P1 P2 ����ӧQ�D����� OK
    // onTrigger�b�P�_���a�b�̭��ɷ|��������D �O�i�H�վ㪺���ε{��
    // �{���ثe �p�G P1 P2 �b�P�Ӫ���M��O�i�H���Ƨ��key ��  (�p�G�n���� " �Q��� " ��N����䪺�ܤ���A�վ�)
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

    // P1 P2 ���}Trigger�d���P�_�ʵe�^��IDLE
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
