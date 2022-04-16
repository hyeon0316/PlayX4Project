﻿using System;
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
    private int _bombCount;
    private Queue<GameObject> _poolingBomb = new Queue<GameObject>();
    private Queue<GameObject> _poolingEffect = new Queue<GameObject>();

    


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
        Initdata(150, 5, 2); //데이터 입력
        _state = Enemystate.Idle;

    }

    // Update is called once per frame
    void Update()
    {
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
            ++_bombCount;
            _poolingBomb.Enqueue(CreateNewBomb());
            _poolingEffect.Enqueue(CreateNewEffect());
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
    
    private void DropBomb()
    {
        int rand = Random.Range(0, 2);
        if (rand == 1)
        {
            if (_poolingBomb.Count > 0)
            {
                var obj = _poolingBomb.Dequeue();
                obj.transform.position = this.transform.position;
                obj.gameObject.SetActive(true);
            }
            else
            {
                return;
            }
        }

        if (_poolingBomb.Count <= 0)
        {
            _state = Enemystate.Skill;
            StartCoroutine(BombSkillCo());
        }
    }

    private IEnumerator BombSkillCo()
    {
        Animator.SetTrigger("Skill1");
        yield return new WaitForSeconds(Animator.GetCurrentAnimatorStateInfo(0).length);
        //todo: 폭탄 파괴, 꺼낸 데이터를 다시 넣어주기
        Debug.Log("폭발!");
        
        for (int i = 0; i < _bombCount; i++)
        {
            var effectObj = _poolingEffect.Dequeue();
            effectObj.transform.position = GameObject.Find("Bomb").transform.position;
            //todo: 몸에서 바로 나오는 것이 아닌 원을 그리면서 폭탄을 드롭하기
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
            if (Vector3.Distance(PlayerObj.transform.position, this.transform.position) < Attackcrossroad + 0.25f)
            {
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
                _state = Enemystate.Find;
            }
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
