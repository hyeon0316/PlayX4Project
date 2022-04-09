﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bringer : Life, I_hp, I_EnemyControl
{
  static public GameObject PlayerObj;

  private Enemystate Enemystate;

  public Enemystate _enemystate
  {
    get { return Enemystate; }
    set { Enemystate = value; }
  }

  private float _attackDelay;

  public Animator Animator;

  private EnemyAttack _enemyAttack;

  private NavMeshAgent _EnemyNav;

  public float Attackcrossroad;

  private void Awake()
  {
    Initdata(50, 5, 3); //데이터 입력
    Enemystate = Enemystate.Attack;
    PlayerObj = GameObject.Find("Player");
    Animator = this.GetComponentInChildren<Animator>();
    _enemyAttack = this.GetComponentInChildren<EnemyAttack>();
    _EnemyNav = this.GetComponent<NavMeshAgent>();
    _EnemyNav.stoppingDistance = Attackcrossroad;
  }

  private void Update()
  {
    if (Enemystate != Enemystate.Dead)
    {
      if (_attackDelay > 0)
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
      if (Enemystate != Enemystate.Attack)
      {
        if (_attackDelay <= 0)
        {
          Enemystate = Enemystate.Find;
        }
        else
        {
          Enemystate = Enemystate.Idle;
        }
      }
    }
    else
    {
      Enemystate = Enemystate.Idle;
      Animator.SetBool("IsWalk", false);
    }
  }

  public void Fieldofview()
  {
    if (Enemystate == Enemystate.Find)
    {
      if (Vector3.Distance(PlayerObj.transform.position, this.transform.position) < Attackcrossroad + 0.25f)
      {
        if (_attackDelay <= 0)
        {
          _attackDelay = 5f;
          Enemystate = Enemystate.Attack;
          Animator.SetTrigger("Attack");
        }
      }
    }

    if (Enemystate == Enemystate.Attack)
    {
      if ((Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
          && Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f)
      {
        Enemystate = Enemystate.Find;
      }

      if (_attackDelay <= 0)
      {
        Enemystate = Enemystate.Find;
      }
    }
  }

  public bool Gethit(int Cvalue)
  {
    if (Cvalue > 0)
    {
      _attackDelay += 0.5f;
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
    Enemystate = Enemystate.Dead;
    _EnemyNav.isStopped = true;
    _EnemyNav.path.ClearCorners();
    while (true)
    {
      if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Death")
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
    //적의 공격범위 콜리더가 플레이어 콜리더 범위에 들어왔을때 플레이어에게 데미지를 줌
    if (_enemyAttack.IshitPlayer)
    {
      Debug.LogFormat("{0},{1}", this.name, "hit");
      PlayerObj.GetComponent<I_hp>().Gethit(Power);
    }
  }

  public void EnemyMove()
  {
    if (Enemystate == Enemystate.Find)
    {
      _EnemyNav.isStopped = false;
      if (_attackDelay <= 0)
      {

        Animator.SetBool("IsWalk", true); //todo: 처음 공격 시 제자리 걸음 한번 이후 Idle로 가는 버그 수정하기
        _EnemyNav.speed = Speed;
        _EnemyNav.SetDestination(PlayerObj.transform.position);
      }
      else 
      {
        Animator.SetBool("IsWalk", false);
        _EnemyNav.isStopped = true;
        _EnemyNav.path.ClearCorners();
        Enemystate = Enemystate.Idle;
      }

    }
    else
    {
      _EnemyNav.isStopped = true;
      _EnemyNav.path.ClearCorners();

    }

    //적 보는 방향 전환라인, 공격 중일 때는 방향 전환x
    if (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
    {
      Vector3 thisScale = new Vector3(2.5f, 2.5f, 1);
      if (PlayerObj.transform.position.x > this.transform.position.x)
      {
        this.transform.GetChild(0).localScale = new Vector3(-thisScale.x, thisScale.y, thisScale.z);
      }
      else
      {
        this.transform.GetChild(0).localScale = new Vector3(thisScale.x, thisScale.y, thisScale.z);
      }
    }
  }
}
