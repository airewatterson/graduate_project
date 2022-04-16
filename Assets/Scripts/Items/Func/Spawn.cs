using General;
using UnityEngine;

namespace Items.Func
{
    public class Spawn : SingletonMonoBehavior<Spawn>
    {
        [SerializeField] internal GameObject item;
        [SerializeField] internal GameObject player1;

        private void Start()
        {
            player1 = GameManager.Instance.player1;
        }
    }
    
}