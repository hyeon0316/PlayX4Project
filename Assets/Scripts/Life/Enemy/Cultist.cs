using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cultist : Life, I_hp, I_EnemyControl
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

    public GameObject[] FireballMem = new GameObject[10];

    // Start is called before the first frame update
    private void Awake()
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
                if (_attackDelay <= 0) { 
                
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
           
                if(Mathf.Abs(PlayerObj.transform.position.z - this.transform.position.z) < 0.3f) { 
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
            if ((Animator.GetCurrentAnimatorStateInfo(0).IsName("Cultist_Attack") || Animator.GetCurrentAnimatorStateInfo(0).IsName("Cultist_farawayAttack"))
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
            if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Cultist_Death")
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
        Debug.Log("파이어볼생성");

        int index = FireballMemorypool();
        GameObject InsFireball = FireballMem[index];
        InsFireball.SetActive(true);
        InsFireball.transform.position = this.transform.position;
        if(this.transform.GetChild(0).localScale.x < 0)
        {
            InsFireball.transform.rotation = new Quaternion(0,180,0,0);
        }
        InsFireball.GetComponent<FireBall>().Power = Power;
        
    }

    /// <summary>
    /// 메모리풀 사용준비
    /// </summary>
    public int FireballMemorypool()
    {
        int index = -1;

        for(int i = 0; i < FireballMem.Length; ++i) { 
            if(!FireballMem[i].activeSelf)
            {
                index = i;
                break;
            }
        }
        return index;
    }
    public void EnemyMove()
    {

        if (Enemystate == Enemystate.Find)
        {
            _EnemyNav.isStopped = false;
            if (_attackDelay <= 0) { 
            _EnemyNav.speed = Speed;
                Vector3 navPosition;
            if (Vector3.Distance(this.transform.position, PlayerObj.transform.position) > 3f) {
                  navPosition = PlayerObj.transform.position;
            }
            else
            {
                 navPosition = new Vector3(this.transform.position.x, this.transform.position.y, PlayerObj.transform.position.z);
            }
                _EnemyNav.SetDestination(navPosition);
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
