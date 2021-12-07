using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Timer : MonoBehaviour
{


    // 因為遊戲UI有間隔 所以到時用 textmesh Pro 調整間隔


    public float timerCount = 180; // 時間秒數可調整
    public Text timerText;
    public GameObject failedPenel;

    public Text TimeOverText;

    // Update is called once per frame
    void Start()
    {
        TimeOverText.text = ""; 
    }
    void Update()
    {
        if (timerCount > 0)
        {
            timerCount -= Time.deltaTime;
        }
        else
        {
            timerCount = 0;
            failedPenel.SetActive(true);
        }

        displaytTime(timerCount);

    }

    void displaytTime(float timeToDisplay) 
    {
        if (timeToDisplay < 0) {
            timeToDisplay = 0;
            TimeOverText.text = "Draw";
        }
        float mins = Mathf.FloorToInt(timeToDisplay / 60);
        float secs = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", mins, secs);
    }

   public void restartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void quitGame()
    {
        Application.Quit();
    }


}
