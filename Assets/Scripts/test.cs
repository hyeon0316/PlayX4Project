﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject NavItem;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(NavItem, FindObjectOfType<Player>().transform.position, Quaternion.identity);
    }
}
