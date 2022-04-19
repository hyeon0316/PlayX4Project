using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Demon : Life, I_hp, I_EnemyControl
{
    private Enemystate Enemystate;
    public Enemystate _enemystate
    {
        get { return Enemystate; }
        set { Enemystate = value; }
    }


    static public GameObject PlayerObj;
    private Enemystate _state;
    public Animator Animator;
    private EnemyAttack _enemyAttack;
    private NavMeshAgent _enemyNav;
    public float Attackcrossroad;
    private float _attackDelay;

    private SpriteRenderer _spriteRenderer;
    public Material HitMaterial;
    private Material _defaultMaterial;

    
    public GameObject FireCollect;
    public GameObject BombEffect;
    public GameObject FireBall;
    
    private int _bombCount;
    private Queue<GameObject> _poolingBomb = new Queue<GameObject>();
    private Queue<GameObject> _poolingEffect = new Queue<GameObject>();
    private Queue<GameObject> _poolingFireBall = new Queue<GameObject>();

    private float _teleportTimer;
    
    private float _areaSkillTimer;

    private float _launchSkillTimer;

    private void Awake()
    {
        PlayerObj = GameObject.Find("Player");
        Animator = this.GetComponentInChildren<Animator>();
        _enemyAttack = this.GetComponentInChildren<EnemyAttack>();
        _enemyNav = this.GetComponent<NavMeshAgent>();
        _enemyNav.stoppingDistance = Attackcrossroad;

        _spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        _defaultMaterial = _spriteRenderer.material;
    }

 
    // Start is called before the first frame update
    void Start()
    {
        InitBomb(4);
        InitFireBall(10);
        Initdata(300, 5, 2); //데이터 입력
        _state = Enemystate.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(_state);
        if (_state != Enemystate.Dead)
        {
            if (_attackDelay > 0)
                _attackDelay -= Time.deltaTime;

            LookPlayer();
            ChangeState();
        }
        EnemyMove();
    }

    private void InitBomb(int initCount)
    {
        for (int i = 0; i < initCount; i++)
        {
            _poolingBomb.Enqueue(CreateNewBomb());
            _poolingEffect.Enqueue(CreateNewEffect());
        }
    }

    private void InitFireBall(int initCount)
    {
        for (int i = 0; i < initCount; i++)
        {
            _poolingFireBall.Enqueue(CreateNewFireBall());
        }
    }
    private GameObject CreateNewBomb()
    {
        var newBomb = Instantiate(FireCollect);
        newBomb.name = "Bomb";
        newBomb.gameObject.SetActive(false);

        return newBomb;
    }

    private GameObject CreateNewEffect()
    {
        var newEffect = Instantiate(BombEffect);
        newEffect.name = "Effect";
        newEffect.gameObject.SetActive(false);

        return newEffect;
    }

    private GameObject CreateNewFireBall()
    {
        var newFireBall = Instantiate(FireBall);
        newFireBall.name = "FireBall";
        newFireBall.gameObject.SetActive(false);

        return newFireBall;
    }

    private void ReturnBomb(GameObject bomb)
    {
        bomb.gameObject.SetActive(false);
        _poolingBomb.Enqueue(bomb);
        
    }
    public void ReturnEffect(GameObject effect)
    {
        effect.gameObject.SetActive(false);
        _poolingEffect.Enqueue(effect);
    }

    public void ReturnFireBall(GameObject fireBall)
    {
        fireBall.gameObject.SetActive(false);
        _poolingEffect.Enqueue(fireBall);
    }
    
    private void DropBomb()
    {
        int rand = Random.Range(0, 2);
        if (rand == 1)
        {
            if (_poolingBomb.Count > 0)
            {
                ++_bombCount;
                var obj = _poolingBomb.Dequeue();
                //obj.transform.position = this.transform.position;
                //todo: 폭탄 드롭을 원형을 그리면서 드롭하게 하기
                obj.gameObject.SetActive(true);
                StartCoroutine(DropPosCo(obj));
            }
            else
            {
                return;
            }
        }

        if (_poolingBomb.Count <=0)
        {
            _state = Enemystate.Skill;
            StartCoroutine(BombSkillCo());
        }
    }

    /// <summary>
    /// 폭탄이 자기 위치 기준으로 시작하여 랜덤 방향으로 드롭되도록 하는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator DropPosCo(GameObject bomb)
    {
        float time = 0;
        var pos1 = transform.position;
        var pos = transform.position + Vector3.right *2 + Vector3.down;
        while (time <= 1f)
        {
            time += Time.deltaTime;
            yield return new WaitForFixedUpdate();
            bomb.transform.position = Vector3.Slerp(pos1, pos, time);
        }
    }

    private IEnumerator BombSkillCo()
    {
        Animator.SetTrigger("Skill1");
        yield return new WaitForSeconds(1f);
        Debug.Log("폭발!");
        
        for (int i = 0; i < _bombCount; i++)
        {
            var effectObj = _poolingEffect.Dequeue();
            effectObj.transform.position = GameObject.Find("Bomb").transform.position;
            effectObj.gameObject.SetActive(true);
            
            ReturnBomb(GameObject.Find("Bomb"));
        }
        _bombCount = 0;
    }
    

    private void ChangeState()
    {
        if (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Skill1") && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Skill2") &&
            !Animator.GetCurrentAnimatorStateInfo(0).IsName("Skill3"))
        {
            if (_state != Enemystate.Skill2)
            {
                if (Vector3.Distance(PlayerObj.transform.position, this.transform.position) < Attackcrossroad + 0.25f)
                {
                    _areaSkillTimer += Time.deltaTime;
                    _teleportTimer = 0;
                    if (_attackDelay <= 0)
                    {
                        _state = Enemystate.Attack;
                    }
                    else
                    {
                        _state = Enemystate.Idle;
                    }
                }
                else
                {
                    _teleportTimer += Time.deltaTime;
                    _areaSkillTimer = 0;

                    if (_teleportTimer >= 5f)
                    {
                        this.transform.position = PlayerObj.transform.position;
                    }
                    _state = Enemystate.Find;
                }
            }
        }
        
        if (_areaSkillTimer >= 4f)
        {
            _state = Enemystate.Skill;
            Animator.SetTrigger("Skill2");
            _areaSkillTimer = 0;
            //todo: 가능하면 플레이어가 넉백효과도 받을 수 있는 기능도 구현
        }
    }
    public void EnemyMove()
    {
        if (_state == Enemystate.Find)
        {
            _enemyNav.isStopped = false;
            _enemyNav.speed = Speed;
            _enemyNav.SetDestination(PlayerObj.transform.position);
            Animator.SetBool("IsWalk", true);
        }
        else if (_state == Enemystate.Attack)
        {
            _enemyNav.isStopped = true;
            Animator.SetBool("IsWalk", false);
            Animator.SetTrigger("Attack");
            _attackDelay = 3f;
        }
        else if (_state == Enemystate.Idle)
        {
            _enemyNav.isStopped = true;
            Animator.SetBool("IsWalk", false);
        }
        else if (_state == Enemystate.Dead)
        {
            Animator.SetBool("IsWalk", false);
            Animator.SetTrigger("Dead");
            StartCoroutine(DeadAniPlayer());
        }
        else if (_state == Enemystate.Skill)
        {
            Animator.SetBool("IsWalk", false);
            _attackDelay = 3f;
            _enemyNav.isStopped = true;
        }
        else if (_state == Enemystate.Skill2)
        {
            _launchSkillTimer += Time.deltaTime;
            _enemyNav.isStopped = true;
            Animator.SetBool("IsWalk", false);
            Animator.SetTrigger("Fire1");
            Animator.SetTrigger("Fire2");

            if (_launchSkillTimer >= 5f)
            {
                Animator.ResetTrigger("Fire1");
                _launchSkillTimer = 0;
                _state = Enemystate.Find;
            }
            
        }
    }
    
    public bool Gethit(int Cvalue)
    {
        if (_state != Enemystate.Dead)
        {
            if (Cvalue > 0)
            {
                StartCoroutine(HitCo());
                DropBomb();
            }
        }
        HP -= Cvalue;
        
        return CheckLiving();
    }
    
    private IEnumerator HitCo()
    {
        _spriteRenderer.material = HitMaterial;
        yield return new WaitForSeconds(0.2f);
        _spriteRenderer.material = _defaultMaterial;
    }
    
    public bool CheckLiving()
    {
        if (HP <= 0)
        {
            _state = Enemystate.Dead;
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

    
    /// <summary>
    /// 실제 데미지를 주는 함수(공격 애니메이션 따로)
    /// </summary>
    public void EnemyAttack()
    {
        if (_enemyAttack.IshitPlayer)
        {
            Debug.LogFormat("{0},{1}", this.name, "hit");
            PlayerObj.GetComponent<I_hp>().Gethit(Power);
        }
    }

    /// <summary>
    /// 원거리 공격
    /// </summary>
    public void LaunchFireBall()
    {
        var fireBallObj = _poolingFireBall.Dequeue();
        if(this.transform.GetChild(0).localScale.x < 0)
        {
            fireBallObj.transform.rotation = new Quaternion(0,180,0,0);
        }
        else
        {
            fireBallObj.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        fireBallObj.transform.position = GameObject.Find("FirePos").transform.position;
        fireBallObj.gameObject.SetActive(true);
    }
    
    private void LookPlayer()
    {
        if (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Skill1") &&
            !Animator.GetCurrentAnimatorStateInfo(0).IsName("Skill2") && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Skill3"))
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
