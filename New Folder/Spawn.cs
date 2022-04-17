using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public Transform[] SpawnPoints;
    public float spawnTime = 0.1f;
    public GameObject Items;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void SpawnItem()
    {
        int spawnIndex = Random.Range(7, 7);
        Instantiate(Items, SpawnPoints[spawnIndex].position, SpawnPoints[spawnIndex].rotation);
    }
}
