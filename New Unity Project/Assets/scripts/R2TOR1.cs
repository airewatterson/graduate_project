using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R2TOR1 : MonoBehaviour
{
    public Camera cameraP1Pos;
    public Camera cameraP2Pos;
    public Transform ROOM1;
    public Transform ROOM2;
   
    public float moveSp;






    void OnTriggerEnter(Collider col)
    {

        if (col.gameObject.tag == "P1")
        {


            R2toR1_P1();
            Debug.Log("R2toR1_P1ok");

        }

        else if (col.gameObject.tag == "P2")
        {


            R2toR1_P2();
            Debug.Log("R2toR1_P2ok");

        }
    }

    void R2toR1_P1()
    {
        cameraP1Pos.transform.position = Vector3.Lerp(ROOM2.position, ROOM1.position, Time.deltaTime * moveSp);
    }
    void R2toR1_P2()
    {
        cameraP2Pos.transform.position = Vector3.Lerp(ROOM2.position, ROOM1.position, Time.deltaTime * moveSp);
    }
}
