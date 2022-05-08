using System;
using UnityEngine;

namespace MapSet
{
    public class Gate : MonoBehaviour
    {
        [SerializeField]private Animator animator;
        private static readonly int IsCompleted = Animator.StringToHash("isFinish");


        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Player.Input.Player>().isCollected)
            {
                if (CompareTag("Finish"))
                {
                    animator.SetInteger(IsCompleted,1);
                }
                else if (CompareTag("Finish2"))
                {
                    animator.SetInteger(IsCompleted,2);
                }
                else if (CompareTag("Finish3"))
                {
                    animator.SetInteger(IsCompleted,3);
                }
                else
                {
                    animator.SetInteger(IsCompleted,0);
                }
                
            }
        }
    }
}
