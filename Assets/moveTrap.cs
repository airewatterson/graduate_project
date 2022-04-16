using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class moveTrap : MonoBehaviour
{

    NavMeshAgent agent;
    public Transform[] waypoint;
    int pointIndex;
    Vector3 target;


  
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Destination();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, target) < 1)
        {
            IteratePointIndex();
            Destination();
        }
    }

    void Destination()
    {
        target = waypoint[pointIndex].position;
        agent.SetDestination(target);
    }
    void IteratePointIndex()
    {
        pointIndex++;
        if (pointIndex == waypoint.Length)
        {
            pointIndex = 0;
        }
    }







}
