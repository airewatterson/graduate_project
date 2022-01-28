using General;
using UnityEngine;

namespace Items.Func
{
    public class ItemInfo : SingletonMonoBehavior<ItemInfo>
    {
        [SerializeField] internal int itemEffectHp;
        //指定玩家
        internal GameObject player1;
        internal GameObject player2;
        
        //指定控制玩家血量數值
        internal int player1Hp;
        internal int player2Hp;

        private void Start()
        {
            player1 = GameManager.Instance.player1;
            player2 = GameManager.Instance.player2;
            
            player1Hp = GameManager.Instance.player1Hp;
            player2Hp = GameManager.Instance.player2Hp;
        }
    }
}
