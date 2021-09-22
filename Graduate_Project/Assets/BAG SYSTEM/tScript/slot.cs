using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slot : MonoBehaviour
{

    private bag bag;
    private string Player = "Player";
    public int elementCount;
    private void Start()
    {
        bag = GameObject.FindGameObjectWithTag(Player).GetComponent<bag>();
    }

    private void Update()
    {

        if (transform.childCount <= 0) {
            bag.isFull[elementCount] = false;
        }
    }

    public void Dropitem()
    {
        foreach(Transform child in transform)
        {
            child.GetComponent<spawnDrop>().spawnDropitem();

            GameObject.Destroy(child.gameObject);
        }
    }



    
}
