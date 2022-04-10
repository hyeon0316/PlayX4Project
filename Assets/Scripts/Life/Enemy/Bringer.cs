using System;
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
    Initdata(50, 5, 3); //데이터 입력
    Enemystate = Enemystate.Idle;
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

  private void ActiveSkill()
  {
    _attackDelay += 2;
    _EnemyNav.isStopped = true;
    _EnemyNav.path.ClearCorners();
    Animator.SetBool("IsWalk", false);
    Animator.SetTrigger("Skill");
    Invoke("StartTracking",1f);
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
          Animator.SetBool("IsWalk", true); //todo: 처음 공격 시 제자리 걸음 한번 이후 Idle로 가는 버그 수정하기
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
          _attackDelay = 3f;
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
    else if (HP <= _Maxhp / 2)
    {
      ActiveSkill();
      return false;
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
