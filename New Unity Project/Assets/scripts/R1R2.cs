using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R1R2 : MonoBehaviour
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


            R1toR2_P1();
            Debug.Log("R1toR2_P1ok");

        }

        else if (col.gameObject.tag == "P2")
        {


            R1toR2_P2();
            Debug.Log("R1toR2_P2ok");

        }
    }

    void R1toR2_P1()
    {
        cameraP1Pos.transform.position = Vector3.Lerp(ROOM1.position, ROOM2.position, Time.deltaTime * moveSp);
    }
    void R1toR2_P2()
    {
        cameraP2Pos.transform.position = Vector3.Lerp(ROOM1.position, ROOM2.position, Time.deltaTime * moveSp);
    }



}
