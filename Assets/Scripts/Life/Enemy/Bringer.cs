using System;
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
    Initdata(50, 5, 3);//데이터 입력
    Enemystate = Enemystate.Attack;
    PlayerObj = GameObject.Find("Player");
    Animator = this.GetComponentInChildren<Animator>();
    _enemyAttack = this.GetComponentInChildren<EnemyAttack>();
    _EnemyNav = this.GetComponent<NavMeshAgent>();
    _EnemyNav.stoppingDistance = Attackcrossroad;
  }

  private void Update()
  {
    
  }

  public bool Gethit(int Cvalue)
  {
    throw new NotImplementedException();
  }

  public bool CheckLiving()
  {
    if (HP <= 0)
    {
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
    while (true)
    {
      if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Bringer-of-Death")
          && Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
      {
        break;
      }
      yield return new WaitForEndOfFrame();
    }
    Destroy(this.transform.parent.gameObject);
  }

  public void EnemyAttack()
  {
    throw new NotImplementedException();
  }

  public void EnemyMove()
  {
    throw new NotImplementedException();
  }
}
