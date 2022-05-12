using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyIcons
{
    Arrow,
    AttackZ,
    AttackX,
    Jump,
    Shift,
    SkillA,
    SkillS,
    SkillD,
}
public class Tutorial : MonoBehaviour
{
    private Player _player;
    public GameObject[] Keys;
    
    private void Awake()
    {
        _player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        Keys[(int)KeyIcons.Arrow].SetActive(true);
    }

    private void Update()
    {
        TargetPlayer();

        if (Keys[(int) KeyIcons.Arrow].activeSelf)
        {
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                Keys[(int) KeyIcons.Arrow].SetActive(false);
            }
        }
        else if (Keys[(int) KeyIcons.AttackX].activeSelf)
        {
            
        }
        else if (Keys[(int) KeyIcons.AttackX].activeSelf)
        {
            
        }
        else if (Keys[(int) KeyIcons.AttackX].activeSelf)
        {
            
        }
        else if (Keys[(int) KeyIcons.AttackX].activeSelf)
        {
            
        }
        else if (Keys[(int) KeyIcons.AttackX].activeSelf)
        {
            
        }
        
    }

    
    /// <summary>
    /// 플레이어 머리 위에 나타내기 위함
    /// </summary>
    private void TargetPlayer()
    {
        for (int i = 0; i < Keys.Length; i++)
        {
            if (Keys[i].activeSelf)
            {
                Keys[i].transform.position = _player.transform.position + Vector3.up;
                break;
            }
        }
    }
    

}
