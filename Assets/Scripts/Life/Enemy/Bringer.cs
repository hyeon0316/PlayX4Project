﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class Bringer : Life, I_hp, I_EnemyControl
{
  static public GameObject PlayerObj;

  public GameObject Skill;

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

  private bool _canSkill;

  private void Awake()
  {
    Initdata(10,360, 5, 3); //데이터 입력
    Enemystate = Enemystate.Idle;
    PlayerObj = GameObject.Find("Player");
    Animator = this.GetComponentInChildren<Animator>();
    _enemyAttack = this.GetComponentInChildren<EnemyAttack>();
    _EnemyNav = this.GetComponent<NavMeshAgent>();
    _EnemyNav.stoppingDistance = Attackcrossroad;
    _canSkill = true;
  }

  private void Update()
  {
        if (Enemystate != Enemystate.Stop)
        {
            if (Enemystate != Enemystate.Dead)
            {
                if (_attackDelay > 0)
                    _attackDelay -= Time.deltaTime;

                FindPlayer();
                Fieldofview();
                EnemyMove();

                if (_canSkill && HP <= _Maxhp / 2)
                {
                    ActiveSkill();
                }

            }
            if (_EnemyNav.enabled)
                GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
  }

  /// <summary>
  /// 플레이어를 따라다니느 트랩스킬
  /// </summary>
  private void ActiveSkill()
  {
      _attackDelay += 2;
      Animator.SetBool("IsWalk", false);
      Animator.SetTrigger("Skill");
      Invoke("StartTracking", 2f);
      _canSkill = false;
  }

  private void StartTracking()
  {
    StartCoroutine(SkillTrackingCo());
  }

  private void FindPlayer()
  {
    if (Vector3.Distance(PlayerObj.transform.position, this.transform.position) < 8f)
    {
      if (Enemystate != Enemystate.Attack)
      {
        if (_attackDelay <= 0)
        {
          Enemystate = Enemystate.Find;
          Animator.SetBool("IsWalk", true); 
        }
        else
        {
          Enemystate = Enemystate.Idle;
          Animator.SetBool("IsWalk", false);
        }
      }
    }
    else
    {
      Enemystate = Enemystate.Idle;
      Animator.SetBool("IsWalk", false);
    }
  }

  private void Fieldofview()
  {
    if (Enemystate == Enemystate.Find)
    {
      if (Vector3.Distance(PlayerObj.transform.position, this.transform.position) < Attackcrossroad + 0.25f)
      {
        if (_attackDelay <= 0)
        {
          _attackDelay = 1f;
          Enemystate = Enemystate.Attack;
          Animator.SetBool("IsWalk",false);
          Animator.SetTrigger("Attack");
        }
      }
    }
    else if (Enemystate == Enemystate.Attack)
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

  public void SelectHit(AttackHitSoundType type)
  {
    switch (type)
    {
      case AttackHitSoundType.ZHit:
        break;
      case AttackHitSoundType.XHit:
        break;
      case AttackHitSoundType.AHit:
        break;
      case AttackHitSoundType.SHit:
        FindObjectOfType<SoundManager>().Play("Player/DashAttackHit",SoundType.Effect);
        break;
      case AttackHitSoundType.DHit:
        break;
    }
  }
  
  public bool Gethit(float Cvalue , float coefficient)
  {
    if (Cvalue > 0)
    {
            if (_attackDelay < 0.06f)
                _attackDelay += 0.06f;
            Animator.SetTrigger("Hitstart");
        }

    HP -= Cvalue * coefficient;
    
        
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

  private IEnumerator SkillTrackingCo()
  {
    Skill = GameObject.Find("BringerSkill").transform.Find("Bringer_Spell").gameObject;
    for (int i = 0; i < 8; i++)
    {
      Skill.SetActive(true);
      yield return new WaitForSeconds(2f);
      Skill.SetActive(false);
    }
  }

  public IEnumerator DeadAniPlayer()
  {
    Enemystate = Enemystate.Dead;
    Living = false;
    _EnemyNav.enabled = true;
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

    Destroy(this.transform.gameObject, Time.deltaTime);
  }

  public void EnemyAttack(float coefficient)
  {
    //적의 공격범위 콜리더가 플레이어 콜리더 범위에 들어왔을때 플레이어에게 데미지를 줌
    if (_enemyAttack.IshitPlayer)
    {
      Debug.LogFormat("{0},{1}", this.name, "hit");
      PlayerObj.GetComponent<I_hp>().Gethit(Power , coefficient);
    }
  }

  public void EnemyMove()
  {
    if (Enemystate == Enemystate.Find)
    {
      if (_attackDelay <= 0 )
      {
        _EnemyNav.isStopped = false;
        _EnemyNav.speed = Speed;
        _EnemyNav.SetDestination(PlayerObj.transform.position);
      }
      else
      {
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
    if (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Skill"))
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
