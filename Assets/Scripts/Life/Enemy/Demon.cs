using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Demon : Life, I_hp, I_EnemyControl
{
    static public GameObject PlayerObj;
    private Enemystate _state;
    public Animator Animator;
    private EnemyAttack _enemyAttack;
    private NavMeshAgent _enemyNav;
    public float Attackcrossroad;
    private float _attackDelay;
    
    private void Awake()
    {
        Initdata(50, 5, 3); //데이터 입력
        _state = Enemystate.Idle;
        PlayerObj = GameObject.Find("Player");
        Animator = this.GetComponentInChildren<Animator>();
        _enemyAttack = this.GetComponentInChildren<EnemyAttack>();
        _enemyNav = this.GetComponent<NavMeshAgent>();
        _enemyNav.stoppingDistance = Attackcrossroad;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_state != Enemystate.Dead)
        {
            if (_attackDelay > 0)
                _attackDelay -= Time.deltaTime;

            EnemyMove();
        }
    }

    public void EnemyMove()
    {
        throw new System.NotImplementedException();
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
        if (_enemyAttack.IshitPlayer)
        {
            Debug.LogFormat("{0},{1}", this.name, "hit");
            PlayerObj.GetComponent<I_hp>().Gethit(Power);
        }
    }
}
