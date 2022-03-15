using UnityEngine;
using UnityEngine.Serialization;

namespace script
{
    public class Interaction : MonoBehaviour
    {

        [FormerlySerializedAs("UP")] public Animator up;
        //public winCount winCount;

        private static readonly int Up = Animator.StringToHash("up");


        // is Kinematic�n��   �P�_P1 P2�����ʰʵe���� OK  �P�_ P1 P2 ����ӧQ�D����� OK
        // onTrigger�b�P�_���a�b�̭��ɷ|��������D �O�i�H�վ㪺���ε{��
        // �{���ثe �p�G P1 P2 �b�P�Ӫ���M��O�i�H���Ƨ��USB ��  (�p�G�n���� " �Q��� " ��N����䪺�ܤ���A�վ�)
        private void OnTriggerStay(Collider col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                if (!UnityEngine.Input.GetKeyDown(KeyCode.E)) return;
                up.SetBool(Up, true);
                if (GameManager.Instance.player1CollectItem < 4) { 
                    var rollChance = Random.Range(1, 3); // 50 %  ����A�O���������v�N�n
                
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
                    var rollChance = Random.Range(1, 3); // 50 %  ����A�O���������v�N�n

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

        // P1 P2 ���}Trigger�d���P�_�ʵe�^��IDLE
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
