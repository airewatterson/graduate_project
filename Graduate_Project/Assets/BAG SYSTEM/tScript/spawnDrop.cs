using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnDrop : MonoBehaviour
{
    public GameObject item;
    private Transform tplayer;
   


    private string Player = "Player";

    // Start is called before the first frame update
    void Start()
    {
        tplayer = GameObject.FindGameObjectWithTag(Player).transform;
    }



   public void spawnDropitem()
    {
        Vector2 playerPos = new Vector2(tplayer.position.x+ 2f, tplayer.position.y-0.5f);
        Instantiate(item, playerPos, Quaternion.identity);
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
