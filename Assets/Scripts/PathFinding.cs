using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class PathFinding : MonoBehaviour
{
    private NavMeshAgent _navItem;

    private void Awake()
    {
        _navItem = this.GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        _navItem.SetDestination(GameObject.Find("NavCube").transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(_navItem.remainingDistance <=_navItem.stoppingDistance)
            Destroy(this.gameObject);
    }
}
