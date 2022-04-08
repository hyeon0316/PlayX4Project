using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twisted : Life, I_hp, I_EnemyControl
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

    private UnityEngine.AI.NavMeshAgent _EnemyNav;

    public float Attackcrossroad;

    // Start is called before the first frame update
    void Start()
    {
        Initdata(30, 10, 2.5f);//데이터 입력
        Enemystate = Enemystate.Attack;
        PlayerObj = GameObject.Find("Player");
        Animator = this.GetComponentInChildren<Animator>();
        _enemyAttack = this.GetComponentInChildren<EnemyAttack>();
        _EnemyNav = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        _EnemyNav.stoppingDistance = Attackcrossroad;

    }

    // Update is called once per frame
    void Update()
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
                    Animator.SetBool("isWalk", true);
                }
                else
                {
                    Enemystate = Enemystate.Idle;
                    Animator.SetBool("isWalk", false);
                }

            }
        }
        else
        {
            Enemystate = Enemystate.Idle;
            Animator.SetBool("isWalk", false);
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
                    Animator.SetTrigger("AttackTrigger");
                }
            }


        }

        if (Enemystate == Enemystate.Attack)
        {
            if ((Animator.GetCurrentAnimatorStateInfo(0).IsName("Twised-Cultist_Attack"))
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
            if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Twised-Cultist_Death")
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
                _EnemyNav.isStopped = false;
                Animator.SetBool("isRun", true);
                _EnemyNav.speed = Speed;
                _EnemyNav.SetDestination(PlayerObj.transform.position);
            }
            else
            {
                Animator.SetBool("isRun", false);
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


        //적 보는 방향 전환라인
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
