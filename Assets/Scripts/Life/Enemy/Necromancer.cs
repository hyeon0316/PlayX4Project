﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class Necromancer : Life, I_hp, I_EnemyControl
{
    static public GameObject PlayerObj;
    public Animator Animator;
    private NavMeshAgent _enemyNav;

    private int _selectPattern;

    private EnemyspawnManager _enemyspawnManager;

    private bool _canSkill;

    //todo: 잡몹 or 중간보스 소환 시 소환 이펙트 추가하기
    
    private void Awake()
    {
        _canSkill = true;
        _enemyspawnManager = GameObject.Find("EnemyParent").GetComponent<EnemyspawnManager>();
        Initdata(50, 5, 3);
        PlayerObj = GameObject.Find("Player");
        Animator = this.GetComponentInChildren<Animator>();
        _enemyNav = this.GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        EnemyMove();
    }

    private void Update()
    {
        LookPlayer();

        if (_canSkill && HP <= _Maxhp / 2)
        {
            StartCoroutine(SpecialSummon());
            _canSkill = false;
        }
    }

    /// <summary>
    /// 모든 행동패턴
    /// </summary>
    public void EnemyMove()
    {
        _selectPattern = Random.Range(-1, 5);

        switch (_selectPattern)
        {
            case -1:
            case 0:
                Debug.Log("추적");
                _enemyNav.SetDestination(PlayerObj.transform.position);
                _enemyNav.isStopped = false;
                Animator.SetBool("IsWalk", true);
                break;
            case 1:
                Debug.Log("정지");
                _enemyNav.isStopped = true;
                Animator.SetBool("IsWalk", false);
                break;
            case 2:
            case 3:
                Debug.Log("일반 소환");
                StartCoroutine(NomalSummon());
                break;
            case 4:
                if (HP <= _Maxhp / 2)
                {
                    Debug.Log("회복");
                    StartCoroutine(Heal());
                }
                break;
        }
        Invoke("EnemyMove", 2.5f);
    }

    private void LookPlayer()
    {
        if (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Skill"))
        {
            //todo: 바라보는 방향 설정
            if (PlayerObj.transform.position.x > this.transform.position.x)
            {
                transform.GetComponentInChildren<SpriteRenderer>().flipX = false;
            }
            else
            {
                transform.GetComponentInChildren<SpriteRenderer>().flipX = true;
            }
        }
    }
    
    /// <summary>
    /// 일반몬스터 4마리 중 랜덤 소환
    /// </summary>
    private IEnumerator NomalSummon()
    {
        _enemyNav.isStopped = true;
        Animator.SetBool("IsWalk", false);
        Animator.SetTrigger("Skill2");
        yield return new WaitForSeconds(Animator.GetCurrentAnimatorStateInfo(0).length);
        _enemyspawnManager.Spawn(Random.Range(0, 4));
        Animator.SetBool("IsWalk", true);
    }

    /// <summary>
    /// 중간보스 소환(한번만 사용 예정)
    /// </summary>
    private IEnumerator SpecialSummon()
    {
        CancelInvoke("EnemyMove");
        _enemyNav.isStopped = true;
        Animator.SetBool("IsWalk", false);
        Animator.SetTrigger("Skill1");
        yield return new WaitForSeconds(Animator.GetCurrentAnimatorStateInfo(0).length);
        _enemyspawnManager.Spawn(4);
        Debug.Log("특수소환!");
        Invoke("EnemyMove", 3f);
    }

    private IEnumerator Heal()
    {
        _enemyNav.isStopped = true;
        Animator.SetBool("IsWalk", false);
        Animator.SetTrigger("Skill3");
        yield return new WaitForSeconds(Animator.GetCurrentAnimatorStateInfo(0).length);
        //todo: 자신 체력 회복(전체적인 밸런스 정해지면 그때 수치 기입)
    }
    
    public bool Gethit(int Cvalue)
    {
        if (Cvalue > 0)
        {
            Animator.SetTrigger("Hit");
        }
        HP -= Cvalue;
        
        return CheckLiving();
    }

    public bool CheckLiving()
    {
        if (HP <= 0)
        {
            Animator.SetTrigger("Dead");
            StartCoroutine(DeadAniPlayer());
            return true;
        }
       
        else
            return false;
    }
    public IEnumerator DeadAniPlayer()
    {
        _enemyNav.isStopped = true;
        while (true)
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Dead")
                && Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        Destroy(this.transform.gameObject);
    }
    
    

    public void EnemyAttack()
    {
        throw new System.NotImplementedException();
    }

   
}
