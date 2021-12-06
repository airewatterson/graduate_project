using General;
using Redo;
using UnityEngine;

namespace Items.Func
{
    public class ItemInfo : SingletonMonoBehavior<ItemInfo>
    {
        [SerializeField] internal int itemEffectHp;
        //指定玩家
        internal GameObject Player1;
        internal GameObject Player2;
        
        //指定控制玩家血量數值
        internal int Player1Hp;
        internal int Player2Hp;

        private void Start()
        {
            Player1 = GameManager.Instance.player1;
            Player2 = GameManager.Instance.player2;
            
            Player1Hp = GameManager.Instance.player1Hp;
            Player2Hp = GameManager.Instance.player2Hp;
        }
    }
}
