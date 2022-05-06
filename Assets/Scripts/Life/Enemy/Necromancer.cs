﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class Necromancer : Life, I_hp, I_EnemyControl
{

    private Enemystate Enemystate;
    public Enemystate _enemystate {
        get { return Enemystate; }
        set { Enemystate = value; }
    }


    static public GameObject PlayerObj;
    public Animator Animator;
    private NavMeshAgent _enemyNav;

    private int _selectPattern;

    private bool _canSpecialSummon;

    public GameObject Portal;

    public static bool IsSkill;
    public static bool IsCutScene;

    private string[] _eventSentences;
    private void Awake()
    {
        _canSpecialSummon = true;
        Initdata(100,50, 5, 3);
        PlayerObj = GameObject.Find("Player");
        Animator = this.GetComponentInChildren<Animator>();
        _enemyNav = this.GetComponent<NavMeshAgent>();
        Portal = GameObject.Find("PortalParent");
        _eventSentences = new string[] {"죽여버리겠다!!", "Stop"};
    }

    private void Start()
    {
        EnemyMove();
    }

    private void Update()
    {
        LookPlayer();

        if (_canSpecialSummon && HP <= _Maxhp / 2)
        {
            StartCoroutine(SpecialSummon());
            _canSpecialSummon = false;
        }
    }

    /// <summary>
    /// 모든 행동패턴
    /// </summary>
    public void EnemyMove()
    {
        if (IsCutScene)
        {
            _selectPattern = 1;
        }
        else
        {
            _selectPattern = Random.Range(-1, 5);
        }

        switch (_selectPattern)
        {
            case -1:
            case 0:
                Debug.Log("추적");
                _enemyNav.SetDestination(PlayerObj.transform.position);
                _enemyNav.isStopped = false;
                Animator.SetBool("IsWalk", true);
                break;
            case 1:
                Debug.Log("정지");
                _enemyNav.isStopped = true;
                Animator.SetBool("IsWalk", false);
                break;
            case 2:
            case 3:
                Debug.Log("일반 소환");
                StartCoroutine(NomalSummon());
                break;
            case 4:
                if (HP <= _Maxhp / 2)
                {
                    Debug.Log("회복");
                    StartCoroutine(Heal());
                }
                break;
        }
        Invoke("EnemyMove", 2.5f);
    }

    private void LookPlayer()
    {
        if (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Skill"))
        {
            //todo: 바라보는 방향 설정
            if (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Stop") && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
            {
                if (PlayerObj.transform.position.x > this.transform.position.x)
                {
                    transform.GetComponentInChildren<SpriteRenderer>().flipX = false;
                }
                else
                {
                    transform.GetComponentInChildren<SpriteRenderer>().flipX = true;
                }
            }
        }
    }
    
    /// <summary>
    /// 일반몬스터 4마리 중 랜덤 소환
    /// </summary>
    private IEnumerator NomalSummon()
    {
        _enemyNav.isStopped = true;
        Animator.SetBool("IsWalk", false);
        Animator.SetTrigger("Skill2");
        yield return new WaitForSeconds(Animator.GetCurrentAnimatorStateInfo(0).length);
        Portal.transform.GetChild(0).gameObject.SetActive(true);
        Animator.SetBool("IsWalk", true);
    }

    /// <summary>
    /// 중간보스 소환(한번만 사용 예정)
    /// </summary>
    private IEnumerator SpecialSummon()
    {
        IsSkill = true;
        CancelInvoke("EnemyMove");
        _enemyNav.isStopped = true;
        Animator.SetBool("IsWalk", false);
        Animator.SetTrigger("Skill1");
        yield return new WaitForSeconds(Animator.GetCurrentAnimatorStateInfo(0).length);
        Portal.transform.GetChild(0).gameObject.SetActive(true);
        Debug.Log("특수소환!");
        Invoke("EnemyMove", 3f);
    }

    private IEnumerator Heal()
    {
        _enemyNav.isStopped = true;
        Animator.SetBool("IsWalk", false);
        Animator.SetTrigger("Skill3");
        yield return new WaitForSeconds(Animator.GetCurrentAnimatorStateInfo(0).length);
        //todo: 자신 체력 회복(전체적인 밸런스 정해지면 그때 수치 기입)
    }

    public bool Gethit(float Cvalue, float coefficient)
    {
        if (HP > 0)
        {
            if (Cvalue > 0)
            {
                Animator.SetTrigger("Hit");
            }
            HP -= Cvalue * coefficient;
            return CheckLiving();
        }
        return false;
    }

    public bool CheckLiving()
    {
        if (HP <= 0)
        {
            Animator.SetTrigger("Stop");
            CancelInvoke("EnemyMove");
            StartCoroutine(DeadAniPlayer());
            return true;
        }
        else
            return false;
    }
    public IEnumerator DeadAniPlayer()
    {
        _enemyNav.enabled = true;
        _enemyNav.isStopped = true;
        FindObjectOfType<DialogueManager>().OnDialogue(_eventSentences);
        FindObjectOfType<DialogueManager>().TalkPanel.transform.position = GameObject.Find("Demon_Page1").transform.position + new Vector3(0.7f, 0.7f, 0);
        yield return new WaitForSeconds(3f);
        FindObjectOfType<DialogueManager>().TalkPanel.SetActive(false);
        Animator.SetTrigger("Dead");
        while (true)
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Dead")
                && Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                GameObject boss2 = Resources.Load<GameObject>("Prefabs/Enemy/Demon_Page2");
                GameObject go = Instantiate(boss2, this.transform.position, this.transform.rotation);
                go.transform.parent = GameObject.Find("EnemyPos_Boss").transform;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        Destroy(this.transform.gameObject, Time.deltaTime);
    }
    
    

    public void EnemyAttack(float coefficient)
    {
        throw new System.NotImplementedException();
    }

   
}
