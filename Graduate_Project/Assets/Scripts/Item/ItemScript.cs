using System;
using System.Collections;
using General;
using Input;
using UnityEngine;

namespace Item
{
    public class ItemScript : SingletonMonoBehavior<ItemScript>
    {
        private Player _player;
        public GameObject effect;
        private Transform _player1;
        
        //HP
        public int hp;
        
        //LandMine
        public GameObject landMine;
        public GameObject awaitFlag;
        

        private void Start()
        {
            _player1 = GameObject.FindGameObjectWithTag("Player").transform;
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            hp = _player.health;
        }

        private void Update()
        {
            hp = _player.health;
        }

        public void Use()
        {
            Instantiate(effect, _player1.position, Quaternion.identity);
            Destroy(gameObject);
        }


        public void Healing()
        {
            hp++;
            Debug.Log(Player.Instance.health);
            Destroy(gameObject);
        }

        public void LandMine()
        {
            Instantiate(awaitFlag, _player1.position, Quaternion.identity);
            
            if (Instantiate(awaitFlag, _player1.position, Quaternion.identity))
            {
                Invoke(nameof(AwaitSpawn),3);
            }

            Instantiate(landMine, awaitFlag.transform.position, Quaternion.identity);

        }

        private IEnumerator AwaitSpawn()
        {
            yield return new WaitForSecondsRealtime(3);
            Destroy(gameObject);
        }
    }
}
