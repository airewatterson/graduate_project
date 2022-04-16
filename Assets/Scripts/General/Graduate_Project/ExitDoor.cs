using UnityEngine;

namespace General.Graduate_Project
{
    public class ExitDoor : SingletonMonoBehavior<ExitDoor>
    {
        private GameManager _gameManager;

        private void Start()
        {
            _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _gameManager.isPlayer1Collected)
            {
                //應該很好懂
                _gameManager.player1StatusText.text = "Win";
                _gameManager.player1Status.SetActive(true);
                Time.timeScale = 0;
            }
            
            else
            {
                _gameManager.player1StatusText.text = "";
                _gameManager.player1Status.SetActive(false);
            }
        }
    }
}