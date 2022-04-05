﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//근거리 적 스크립트
public class Enemy : Life,I_hp,I_EnemyControl
{
    static public GameObject PlayerObj;

    public Enemystate Enemystate;

    public Enemystate _enemystate {
        get { return Enemystate; }
        set { Enemystate = value; }
    }

    private float _attackDelay;

    public Animator Animator;

    private EnemyAttack _enemyAttack;

    private NavMeshAgent _EnemyNav;

    public float Attackcrossroad = 0;
    public void Awake()
    {
        Initdata(50, 5, 3);//데이터 입력
        Enemystate = Enemystate.Attack;
        PlayerObj = GameObject.Find("Player");
        Animator = this.GetComponentInChildren<Animator>();
        _enemyAttack = this.GetComponentInChildren<EnemyAttack>();
        _EnemyNav = this.GetComponent<NavMeshAgent>();
        Attackcrossroad = 0.6f;
    }

    public void Update()
    {
        if(Enemystate != Enemystate.Dead) { 
        if(_attackDelay > 0)
                _attackDelay -= Time.deltaTime;

        FindPlayer();
        Fieldofview();
        EnemyMove();
        }
    }



    public void FindPlayer()
    {
        if (Vector3.Distance(PlayerObj.transform.position, this.transform.position) < 5f)
        {
            if(Enemystate != Enemystate.Attack)
            {
                Enemystate = Enemystate.Find;
                Animator.SetBool("isRun", true);
            }
        }
        else
        {
            Enemystate = Enemystate.Idle;
            Animator.SetBool("isRun", false);
        }
    }

    public void Fieldofview()
    {
        if(Enemystate == Enemystate.Find)
        {
            if(Vector3.Distance(PlayerObj.transform.position,this.transform.position) < Attackcrossroad+0.25f)
            {
                if(_attackDelay <= 0) {
                    _attackDelay = 4f;
                    Enemystate = Enemystate.Attack;
                Animator.SetTrigger("AttackTrigger");
                }
            }
        }

        if(Enemystate == Enemystate.Attack)
        {
            if(Animator.GetCurrentAnimatorStateInfo(0).IsName("Assassin_Attack")
                && Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f)
            {
                Enemystate = Enemystate.Find;
            }
        }
    }

   

    public bool Gethit(int Cvalue)
    {
        if(Cvalue > 0)
        {
            if(Enemystate != Enemystate.Attack)
                Animator.SetTrigger("Hit");
        }

        HP -= Cvalue;

        return CheckLiving();
    }

    public bool CheckLiving()
    {

        if (HP <= 0) {
            Animator.SetBool("Dead", true);
            StartCoroutine(DeadAniPlayer());
            return true;
        }
        else
            return false;
    }

    public IEnumerator DeadAniPlayer()
    {
        Enemystate = Enemystate.Dead;
        _EnemyNav.isStopped = true;
        _EnemyNav.path.ClearCorners();
        while (true){
            if(Animator.GetCurrentAnimatorStateInfo(0).IsName("Assassin_Death")
                &&Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        Destroy(this.transform.parent.gameObject);
        yield return 0;
    }

    /// <summary>
    /// 적 공격관련 스크립트
    /// </summary>
    public void EnemyAttack()
    {
        if (_enemyAttack.IshitPlayer&& _attackDelay <= 0)
        {
           
            PlayerObj.GetComponent<I_hp>().Gethit(Power);
        }
    }
    /// <summary>
    /// 적 행동 관련 스크립트
    /// </summary>
    public void EnemyMove()
    {
         
            if(Enemystate == Enemystate.Find) {
            _EnemyNav.isStopped = false;
                if (_attackDelay <= 0) {
                _EnemyNav.speed = Speed;
                    _EnemyNav.SetDestination(PlayerObj.transform.position);
                }
                else
                {
                Vector3 position = new Vector3(this.transform.position.x + (this.transform.position.x / 0.5f - PlayerObj.transform.position.x)
                    , this.transform.position.y,
                    this.transform.position.z + (this.transform.position.z / 0.5f - PlayerObj.transform.position.z));
                _EnemyNav.speed = Speed / 2;
;               _EnemyNav.SetDestination(position);
                }

            }
            else
            {
                _EnemyNav.isStopped = true;
                _EnemyNav.path.ClearCorners();
            
            }
        
        
        //적 보는 방향 전환라인
        Vector3 thisScale = new Vector3(2.5f, 2.5f, 1);
        if (_EnemyNav.pathEndPosition.x > this.transform.position.x)
        {
            
            this.transform.GetChild(0).localScale = new Vector3(-thisScale.x, thisScale.y, thisScale.z);
        }
        else
        {
            this.transform.GetChild(0).localScale = new Vector3(thisScale.x, thisScale.y, thisScale.z);
        }
    }

  
   

}
