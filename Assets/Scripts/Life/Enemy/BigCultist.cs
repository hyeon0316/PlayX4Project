using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BigCultist : Life, I_hp, I_EnemyControl
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
        Initdata(3,160, 10, 2);//데이터 입력
        Enemystate = Enemystate.Attack;
        PlayerObj = GameObject.Find("Player");
        Animator = this.GetComponentInChildren<Animator>();
        _enemyAttack = this.GetComponentInChildren<EnemyAttack>();
        _EnemyNav = this.GetComponent<NavMeshAgent>();
        _EnemyNav.stoppingDistance = Attackcrossroad;
    }

    // Update is called once per frame
    void Update()
    {
        if(Enemystate != Enemystate.Stop) {
            Animator.SetBool("Hitstop", false);
            if (Enemystate != Enemystate.Dead )
            {
                if (_attackDelay > 0)
                    _attackDelay -= Time.deltaTime;

                FindPlayer();
                Fieldofview();
                EnemyMove();
            }
            if (_EnemyNav.enabled)
                GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
       
    }

    public void FindPlayer()
    {
        if (Vector3.Distance(PlayerObj.transform.position, this.transform.position) < 5f)
        {
            if (Enemystate != Enemystate.Attack && _attackDelay <= 0)
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
        if (Enemystate == Enemystate.Find)
        {
            if (Vector3.Distance(PlayerObj.transform.position, this.transform.position) < Attackcrossroad + 0.25f)
            {
                Animator.SetBool("isRun", false);
                if (_attackDelay <= 0)
                {
                    _attackDelay = 4f;
                    Enemystate = Enemystate.Attack;
                    Animator.SetTrigger("AttackTrigger");
                }
            }
            else
            {
                Animator.SetBool("isRun", true);
            }
          
        }

        if (Enemystate == Enemystate.Attack)
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Big-Cultist_Attack")
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

    public bool Gethit(float Cvalue, float coefficient)
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
            Animator.SetBool("Dead", true);
            StartCoroutine(DeadAniPlayer());
            return true;
        }
        else
            return false;
    }

    public IEnumerator DeadAniPlayer()
    {
        Living = false;
        Enemystate = Enemystate.Dead;
        _EnemyNav.enabled = true;
        _EnemyNav.isStopped = true;
        _EnemyNav.path.ClearCorners();
        while (true)
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Big-Cultist_Death")
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
       
        if (_enemyAttack.IshitPlayer )
        {
            Debug.LogFormat("{0},{1}", this.name, "hit");
            PlayerObj.GetComponent<Player>().KnockBack(this.transform.position);
            PlayerObj.GetComponent<I_hp>().Gethit(Power,coefficient);
            
        }
    }

    public void EnemyMove()
    {

        if (Enemystate == Enemystate.Find && _attackDelay <= 0)
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

        }


        //적 보는 방향 전환라인
        Vector3 thisScale = new Vector3(2.5f, 2.5f, 1);
        if (_EnemyNav.pathEndPosition.x > this.transform.position.x)
        {

            this.transform.GetChild(0).localScale = new Vector3(-thisScale.x, thisScale.y, thisScale.z);
        }
        else if(_EnemyNav.pathEndPosition.x < this.transform.position.x)
        {
            this.transform.GetChild(0).localScale = new Vector3(thisScale.x, thisScale.y, thisScale.z);
        }
    }


}
