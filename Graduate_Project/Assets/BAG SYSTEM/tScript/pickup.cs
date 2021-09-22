using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour

{
    private bag bag;
    public GameObject itemBtn;
    private string Player = "Player";
   
    void Start()
    {
        bag = GameObject.FindGameObjectWithTag(Player).GetComponent<bag>();
        
    }


    private void OnTriggerEnter2D(Collider2D col2d) //碰到物品撿起來
    {
        if (col2d.CompareTag(Player))
        {
            for(int i = 0; i < bag.slots.Length; i++)
            {
                if (bag.isFull[i] == false) //判斷包包是否滿了
                {      
                    bag.isFull[i] = true;
                    Instantiate(itemBtn , bag.slots[i].transform,false);
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }



  
}
