using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyIcons
{
    Arrow,
    Jump,
    SkillA,
    SkillS,
    SkillD,
}
public class Tutorial : MonoBehaviour
{
    private Player _player;
    public GameObject UICanvas;
    
    private void Awake()
    {
        _player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        UICanvas.transform.Find("Arrows").gameObject.SetActive(true);
    }

    private void Update()
    {
        
    }

   
    
}
