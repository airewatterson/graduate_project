using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class winCount : MonoBehaviour
{
    public Text keyP1;
    public Text keyP2;
    public int keyP1Count;
    public int keyP2Count;

    public Text winText1;
    public Text winText2;
    // Start is called before the first frame update
    void Start()
    {
        keyP1Count = 0;
        keyP2Count = 0;
        winText1.text = "";
        winText2.text = "";
            
    }

    // Update is called once per frame
    void Update()
    {
        keyP1.text = keyP1Count.ToString();
        keyP2.text = keyP2Count.ToString();
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "P1")
        {
            if (keyP1Count == 4&&keyP2Count < 4) //勝利為4
            {
                Debug.Log("P1 win");
                winText1.text = "Victory!!!";
                winText2.text = "Almost!!!";
            }
        }
        else if (col.gameObject.tag == "P2")
        {
            if (keyP2Count == 4&& keyP1Count < 4) //勝利為4
            {
                Debug.Log("2 win");

                winText2.text = "Victory!!!";
                winText1.text = "Almost!!!";
            }
        }
    }


}
