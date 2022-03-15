using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCtrl : MonoBehaviour
{
    public Transform trans;
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.transform.position = trans.position;
    }
}
