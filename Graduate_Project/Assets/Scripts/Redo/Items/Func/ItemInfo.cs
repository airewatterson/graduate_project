using System;
using General;
using UnityEngine;

namespace Redo.Items
{
    public class ItemInfo : SingletonMonoBehavior<ItemInfo>
    {
        [SerializeField] internal int itemEffectHp;
        //指定玩家
        private GameObject _player1;
        private GameObject _player2;
        
        //指定控制玩家血量數值
        internal int Player1Hp;
        internal int Player2Hp;

        private void Start()
        {
            _player1 = GameManager.Instance.player1;
            _player2 = GameManager.Instance.player2;
            
            Player1Hp = GameManager.Instance.player1Hp;
            Player2Hp = GameManager.Instance.player2Hp;
        }
    }
}
