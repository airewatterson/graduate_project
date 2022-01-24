using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCtrl : MonoBehaviour
{
    bool isOpen = false;
    Animator ani;

    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "player")
        {
            isOpen = true; 
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            isOpen = false;
        }
    }
}
