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
                _gameManager.player2StatusText.text = "Lose";
                _gameManager.player2Status.SetActive(true);
                Time.timeScale = 0;
            }
            else if (other.CompareTag("Player2") && _gameManager.isPlayer2Collected)
            {
                _gameManager.player1StatusText.text = "Lose";
                _gameManager.player1Status.SetActive(true);
                _gameManager.player2StatusText.text = "Win";
                _gameManager.player2Status.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                _gameManager.player1StatusText.text = "";
                _gameManager.player1Status.SetActive(false);
                _gameManager.player2StatusText.text = "";
                _gameManager.player2Status.SetActive(false);
            }
        }
    }
}