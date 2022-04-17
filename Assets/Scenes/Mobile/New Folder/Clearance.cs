using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clearance : MonoBehaviour
{
    public Text wintext;
    private void Start()
    {
        wintext = gameObject.GetComponent<Text>();
        wintext.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (key = 4)
        {
            wintext.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
