using System;
using UnityEngine;

namespace MapSet
{
    public class Gate : MonoBehaviour
    {
        [SerializeField]private Animator animator;
        

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Player.Input.Player>().isCollected)
            {
                animator.SetBool("isCompleted",true);
            }
        }
    }
}
