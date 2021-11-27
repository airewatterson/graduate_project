using General;
using Input;
using UnityEngine;

namespace Item
{
    public class ItemScript : SingletonMonoBehavior<ItemScript>
    {
        public GameObject effect;
        private Transform _player1;
        
        
        private void Start()
        {
            _player1 = GameObject.FindGameObjectWithTag("Player").transform;
        }

        public void Use()
        {
            Instantiate(effect, _player1.position, Quaternion.identity);
            Destroy(gameObject);
        }


        public void Healing()
        {
            Player.Instance.health++;
            Destroy(gameObject);
            Debug.Log(Player.Instance.health);
        }
    }
}
