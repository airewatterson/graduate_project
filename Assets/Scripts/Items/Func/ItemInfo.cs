using General;
using UnityEngine;

namespace Items.Func
{
    public class ItemInfo : SingletonMonoBehavior<ItemInfo>
    {
        [SerializeField] internal int itemEffectHp;
        //指定玩家
        private GameObject _player1;

        //指定控制玩家血量數值
        internal int Player1Hp;

        private void Start()
        {
            _player1 = GameManager.Instance.player1;

            Player1Hp = GameManager.Instance.player1Hp;
        }
    }
}
