using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace General.Graduate_Project
{
    
    //From：http://www.cg.com.tw/UnityCSharp/Content/Timer.php
    public class Timer : SingletonMonoBehavior<Timer>
    {
        [FormerlySerializedAs("m_seconds")] public int mSeconds;                 //倒數計時經換算的總秒數

        [FormerlySerializedAs("m_min")] public int mMin;              //用於設定倒數計時的分鐘
        [FormerlySerializedAs("m_sec")] public int mSec;              //用於設定倒數計時的秒數

        [FormerlySerializedAs("m_timer")] public Text mTimer;           //設定畫面倒數計時的文字
        [FormerlySerializedAs("m_gameOver")] public GameObject mGameOver;  //設定 GAME OVER 物件

        [SerializeField] private TextMeshProUGUI warning;//
        private AudioSource _aud;//

        [SerializeField] private GameObject enemyPolice;
        [SerializeField] private GameObject enemyBoss;

        private int _enemyCount;
        private void Start()
        {
            StartCoroutine(Countdown());//呼叫倒數計時的協程
        }

        private void Update()
        {
            #region EventTimeline

            switch (mMin)
            {
                #region PoliceSpawnWarning

                case 3 when mSec == 53:
                    warning.text = "Enemy spawn in 3 seconds";
                    break;
                case 3 when mSec == 52:
                    warning.text = "Enemy spawn in 2 seconds";
                    break;
                case 3 when mSec == 51:
                    warning.text = "Enemy spawn in 1 seconds";
                    break;

                #endregion
                
                case 3 when mSec == 50 && _enemyCount < 1:
                    warning.text = "";
                    SpawnPolice();
                    break;

                #region BossWarning

                case 2 when mSec == 03:
                    warning.text = "Enemy spawn in 3 seconds";
                    break;
                case 2 when mSec == 02:
                    warning.text = "Enemy spawn in 2 seconds";
                    break;
                case 2 when mSec == 01:
                    warning.text = "Enemy spawn in 1 seconds";
                    break;

                #endregion
                
                case 2 when mSec == 00 && _enemyCount < 2:
                    warning.text = "";
                    SpawnBoss();
                    break;
            }

            #endregion
            
        }

        private IEnumerator Countdown()
        {
            mTimer.text = $"{mMin:00}  {mSec:00}";
            mSeconds = (mMin * 60) + mSec;       //將時間換算為秒數

            while (mSeconds > 0)                   //如果時間尚未結束
            {
                yield return new WaitForSeconds(1); //等候一秒再次執行

                mSeconds--;                        //總秒數減 1
                mSec--;                            //將秒數減 1

                switch (mSec < 0)
                {
                    //如果秒數為 0 且分鐘大於 0
                    case true when mMin > 0:
                        mMin -= 1;                     //先將分鐘減去 1
                        mSec = 59;                     //再將秒數設為 59
                        break;
                    //如果秒數為 0 且分鐘大於 0
                    case true when mMin == 0:
                        mSec = 0;                      //設定秒數等於 0
                        break;
                }
                mTimer.text = $"{mMin:00}  {mSec:00}";
            }

            yield return new WaitForSeconds(1);   //時間結束時，顯示 00:00 停留一秒
            mGameOver.SetActive(true);           //時間結束時，畫面出現 GAME OVER
            Time.timeScale = 0;                   //時間結束時，控制遊戲暫停無法操作
        }


        #region EventHandler

        private void SpawnPolice()
        {
            var xPos = Random.Range(5, 9);
            var zPos = Random.Range(-7, 6);
            Debug.Log("spawn");
            Instantiate(enemyPolice, new Vector3(xPos,1.02f,zPos), Quaternion.identity);
            FindObjectOfType<AudioManager>().Play("Warning");
            _enemyCount++;
        }

        private void SpawnBoss()
        {
            var xPos = Random.Range(6, 9);
            var zPos = Random.Range(-7, 6);
            Debug.Log("spawn");
            Instantiate(enemyBoss, new Vector3(xPos,1.02f,zPos), Quaternion.identity);
            FindObjectOfType<AudioManager>().Play("Warning");
            _enemyCount++;
        }

        #endregion
        
        
    }
    
}