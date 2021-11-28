using System;
using System.Collections;
using General;
using Input;
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
        [SerializeField] private int player1Hp;
        [SerializeField] private int player2Hp;
        private TextMeshProUGUI _player1HpOnUi;
        private TextMeshProUGUI _player2HpOnUi;
        
        //LandMine
        public GameObject landMine;
        public GameObject awaitFlag;
        

        private void Start()
        {
            player1Hp = 3;
            player2Hp = 3;
            _player1 = GameObject.FindGameObjectWithTag("Player").transform;
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            _player1HpOnUi = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
            _player2HpOnUi = GameObject.Find("Text2").GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            //_player1HpOnUi.text = player1Hp.ToString();
            
        }

        public void Use()
        {
            Instantiate(effect, _player1.position, Quaternion.identity);
            Destroy(gameObject);
        }


        public void Healing()
        {
            if (GameObject.Find("Player1").GetComponent<Player>())
            {
                player1Hp++;
                _player1HpOnUi.text = player1Hp.ToString(); 
                Debug.Log(player1Hp);
                Destroy(gameObject);
            }
            else if (GameObject.Find("Player2").GetComponent<ForPlayer2>())
            {
                player2Hp++;
                _player2HpOnUi.text = player2Hp.ToString(); 
                Debug.Log(player2Hp); 
                Destroy(gameObject);
            }
            
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
