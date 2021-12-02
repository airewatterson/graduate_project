using System;
using System.Collections;
using General;
using Input;
using Old.Input;
using TMPro;
using UnityEngine;

namespace Item
{
    public class ItemScript : SingletonMonoBehavior<ItemScript>
    {
        private Player _player;
        public GameObject effect;
        private Transform _player1;
        
        //HP
        private int player1Hp = 3;
        private TextMeshProUGUI _player1HpOnUi;

        //LandMine
        public GameObject landMine;
        public GameObject awaitFlag;
        

        private void Start()
        {
            _player1 = GameObject.FindGameObjectWithTag("Player").transform;
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            _player1HpOnUi = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            
        }

        public void Use()
        {
            Instantiate(effect, _player1.position, Quaternion.identity);
            Destroy(gameObject);
        }


        public void Healing()
        {
            player1Hp++;
            _player1HpOnUi.text = player1Hp.ToString(); 
            Debug.Log(player1Hp); 
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
