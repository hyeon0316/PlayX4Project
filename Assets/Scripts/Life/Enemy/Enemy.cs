﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//근거리 적 스크립트
public class Enemy : Life,I_hp,I_EnemyControl
{
    public Enemystate _enemystate {
        get { return _enemystate; }
        set { _enemystate = value; }
    }

    public void Awake()
    {
        Initdata(0, 0, 0);//데이터 입력
    }



    public bool Gethit(int Cvalue)
    {

        HP -= Cvalue;
        return CheckLiving();
    }

    public bool CheckLiving()
    {

        if (HP <= 0)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 적 이동관련 스크립트
    /// </summary>
    public void EnemyAttack()
    {

    }
    /// <summary>
    /// 적 행동 관련 스크립트
    /// </summary>
    public void EnemyMove()
    {

    }

}
