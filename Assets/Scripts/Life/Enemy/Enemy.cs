using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Life,I_hp,I_EnemyControl
{
    public Enemystate _enemystate {
        get { return _enemystate; }
        set { _enemystate = value; }
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


    public void EnemyAttack()
    {

    }

    public void EnemyMove()
    {

    }

}
