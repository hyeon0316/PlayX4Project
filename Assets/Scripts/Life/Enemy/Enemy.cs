using System.Collections;
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

    private float AttackDelay;

    public Animator Animator;

    private EnemyAttack _enemyAttack;

    private NavMeshAgent _EnemyNav;
    public void Awake()
    {
        Initdata(50, 5, 3);//데이터 입력
        Enemystate = Enemystate.Attack;
        PlayerObj = GameObject.Find("Player");
        Animator = this.GetComponentInChildren<Animator>();
        _enemyAttack = this.GetComponentInChildren<EnemyAttack>();
        _EnemyNav = this.GetComponent<NavMeshAgent>();

    }

    public void Update()
    {
        if(AttackDelay > 0)
        AttackDelay -= Time.deltaTime;
        FindPlayer();
        Fieldofview();
        EnemyMove();
    }



    public void FindPlayer()
    {
        if (Vector3.Distance(PlayerObj.transform.position, this.transform.position) < 3f)
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
            if(Vector3.Distance(PlayerObj.transform.position,this.transform.position) < 0.8f)
            {
                if(AttackDelay <= 0) {
                    AttackDelay = 4f;
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
        while(true){
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
        if (_enemyAttack.IshitPlayer&& AttackDelay <= 0)
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
            _EnemyNav.SetDestination(PlayerObj.transform.position);
           
        }
        else
        {
            _EnemyNav.isStopped = true;
            _EnemyNav.path.ClearCorners();
            
        }

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
