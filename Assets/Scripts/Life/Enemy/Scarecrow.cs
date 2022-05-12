using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : Life,I_hp,I_EnemyControl
{

    public Enemystate Enemystate;

    public Enemystate _enemystate
    {
        get { return Enemystate; }
        set { Enemystate = value; }
    }

    public Animator animator;

    public bool Gethit(float Cvalue, float coefficient)
    {
        if (HP > 0)
        {
            if (Cvalue > 0)
            {

                animator.SetTrigger("Hit");

            }
            //HP -= Cvalue * coefficient;
            return CheckLiving();
        }
        return false;
    }

    public bool CheckLiving()
    {
        if (HP <= 0)
        {
            
            StartCoroutine(DeadAniPlayer());
            return true;
        }
        else
            return false;
    }


    public IEnumerator DeadAniPlayer()
    {
        yield return 0;
    }

    public void EnemyAttack(float coefficient)
    {

    }

     public void EnemyMove()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        Initdata(99999, 10000, 0, 0);
        Enemystate = Enemystate.Idle;
        animator = this.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
