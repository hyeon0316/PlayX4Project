﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;
/*
1.public으로 선언 된 변수는 앞글자 대문자로 시작
2. private는 변수 앞에 "_"붙이고  소문자로 시작
3. 함수 안의 지역변수는 네이밍 규칙 상관없음
4. bool형 변수는 is~ or can~ 로 시작
번외로  나중에 구현해야 할 일을 스크립트에 적어 놓을때 //todo: 할 일(메모장 느낌)
*/


public class Player : Life, I_hp
{
    /// <summary>
    /// 플레이어 상태를 알려주는 enum 변수
    /// </summary>
    public enum PlayerstateEnum { Idle, Attack, Skill,ncSkill, Dead }
    /// <summary>
    /// 플레이어 상태
    /// </summary>
    public PlayerstateEnum Playerstate;

    /// <summary>
    /// 플레이어 총알 메모리풀
    /// </summary>
    public Transform BulletParent;
    public GameObject[] BulletPool;

    /// <summary>
    /// 무언가를 카운트 해야할때 사용할 배열 변수
    /// 0: 무적 ,1: 공격 딜레이 2:대쉬배기 쿨타임 3: 총 쿨타임 4:3번째 스킬 쿨타임
    /// </summary>
    public float[] CountTimeList;
    /// <summary>
    /// 플레이어 애니메이터 변수
    /// </summary>
    public Animator PlayerAnim;

    private Animator _playerEffectAnim;
    /// <summary>
    /// 플레이어 스프라이트 렌더러
    /// </summary>
    private SpriteRenderer _playerSprite;//좌우 이동 시 방향 전환에 쓰일 변수
    /// <summary>
    /// 플레이어 rigidbody
    /// </summary>
    private Rigidbody _rigid;
    /// <summary>
    /// 플레이어가 사용할 UI를 모아둔 부모 객체, 자식을 찾아서 사용한다.
    /// </summary>
    public Transform PlayerUIObj;

    /// <summary>
    /// 플레이어가 날고 있는지 확인
    /// </summary>
    private bool _isFry = false;

    /// <summary>
    /// 계단 확인
    /// </summary>
    private bool _isStair= false;
    /// <summary>
    /// 플레이어가 2번째 공격을 할 수 있는지 확인하는 변수
    /// </summary>
    private bool _isAgainAttack = false;
    /// <summary>
    /// 플레이어가 대화하고 있는지 확인하느변수
    /// </summary>
    public bool IsStop = false;


    private bool _isWall = false;
    /// <summary>
    /// 벽타고 있는중인지 확인
    /// </summary>
    private bool _isWallslide = false;

    private bool _isRoll = false;

    public bool _isLadder = false;

    private float _slowSpeed;
    private float _oriSpeed;

    private float _stepSmooth = 1f;

    private IEnumerator[] enumerators = new IEnumerator[3];

    private RaycastHit _wallslidehit;
    private int _wallslideObject;

    public void Awake()
    {
        //필요한 컴포넌트, 데이터들을 초기화 해준다.
        PlayerAnim = transform.GetChild(0).GetComponent<Animator>();
        _playerEffectAnim = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        _playerSprite = GetComponentInChildren<SpriteRenderer>();
        _rigid = GetComponent<Rigidbody>();
        //스텟을 초기화 해주는 함수.
        Initdata(0,30, 10, 5);
        _oriSpeed = Speed;
        _slowSpeed = _oriSpeed * 0.75f;
        Playerstate = PlayerstateEnum.Idle;
        CountTimeList = new float[5];
        BulletParent = GameObject.Find("Bulletpool").transform;
        BulletPool = new GameObject[12];
        for (int i = 0; i < 12; i++)
        {
            BulletPool[i] = BulletParent.GetChild(i).gameObject;
            BulletPool[i].SetActive(false);
        }
        _wallslideObject = this.gameObject.GetInstanceID();




    }


    private void Update()
    {
        countTime();
        CheckFry();
        if (!_isLadder && !_isWallslide && !SceneManager.GetActiveScene().name.Equals("Town"))
        {
            if (!IsStop && (Playerstate != PlayerstateEnum.Dead && Playerstate != PlayerstateEnum.Skill && Playerstate != PlayerstateEnum.ncSkill))
            {
                PlayerAttack();

            }
            //구르기 상태일때 스킬 사용 불가
            if (!IsStop && (Playerstate != PlayerstateEnum.Dead&&Playerstate != PlayerstateEnum.ncSkill)&& !_isRoll && !_isFry)
            {
                Skill();
            }
        }
    }

    private void FixedUpdate()
    {
        UpDownStair();
        UpdateUI();
        if (!_isLadder)
        {
            if (!SceneManager.GetActiveScene().name.Equals("Town") && !IsStop && (Playerstate != PlayerstateEnum.Dead
                    && Playerstate != PlayerstateEnum.Skill && Playerstate != PlayerstateEnum.ncSkill))
            {
                PlayerJump();
               // WallSlide();
            }

            if (!IsStop && (Playerstate != PlayerstateEnum.Dead&& Playerstate != PlayerstateEnum.Skill && Playerstate != PlayerstateEnum.ncSkill))
            {
                PlayerMove_v1();
            }
        }
        else
        {
            //사다리 타고 있을때
            PlayerAnim.speed = 0;
            LadderMove();
        }
    }
    
    /// <summary>
    /// 2층에서 계단 오를때 판정
    /// </summary>
    private void UpDownStair()
    {
        
        RaycastHit hit;
        Debug.DrawRay(this.transform.position + Vector3.down * 0.7f, transform.TransformDirection(transform.GetChild(0).localScale.x > 0 ? Vector3.right : Vector3.left),
            Color.red);

        if (Physics.Raycast(this.transform.position + Vector3.down * 0.7f,
              transform.TransformDirection(transform.GetChild(0).localScale.x > 0 ? Vector3.right : Vector3.left), out hit, 0.7f, LayerMask.GetMask("Stair")))
        {
            Debug.Log("계단 만남");
            _isStair = true;
        }
        else
        {
            _isStair = false;
        }
        
    }
    
    /// <summary>
    /// 카운트 해야하는 변수들의 시간을 줄여주는 함수
    /// </summary>
    private void countTime()
    {
        for (int i = 0; i < CountTimeList.Length; ++i)
        {
            if (CountTimeList[i] >= 0)
                CountTimeList[i] -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 플레이어의 체력값을 변경할 수 있는 함수
    /// </summary>
    /// <param name="Cvalue"> 양수가 들어오면 데미지를 입고 음수가 들어오면 회복을 할 수 있다.</param>
    /// <returns>플레이어가 사망한다면 true 아니면 false를 반환한다</returns>
    public bool Gethit(float Cvalue ,float coefficient)
    {
        //데미지가 들어오니 무적 카운트와 hit 애니메이션 실행
        if (Cvalue > 0)
        {
            if (HP > 0)
            {
                //플레이어가 무적상태라면 애니메이션과 채력계산을 무시하고 리턴한다.
                if (CountTimeList[0] > 0)
                    return CheckLiving();

                CountTimeList[0] = 1f;
                PlayerAnim.SetTrigger("Hit");
                
                HP -= (Cvalue * coefficient);
                return CheckLiving();
            }
        }
        return false;
    }

    //todo :  KnockBack함수를 Life 클래스를 수정하여 제작할지 테스트후 적용
    /// <summary>
    /// 1번째 인자 반대편으로 너백당하는 함수
    /// </summary>
    /// <param name="EnemyPos">때린 적 위치</param>
    public void KnockBack(Vector3 EnemyPos)
    {
        //무적이 아닐때만 넉백을 입는다.
        if (CountTimeList[0] < 0) {
        StartCoroutine(StopTime(1f));
        Vector3 nomal = (this.transform.position - EnemyPos).normalized;
        Vector3 vector3 = new Vector3(nomal.x, 0.8f, nomal.z);
        _rigid.velocity = Vector3.zero;
        _rigid.velocity = vector3 * 7f;
        }
    }

    IEnumerator StopTime(float time)
    {
        IsStop = true;
        yield return new WaitForSeconds(time);
        IsStop = false;
    }

    /// <summary>
    /// 플레이어가 살아있는지 확인하는 함수, hp 가 0 이하로 떨어진다면 죽는 애니메이션 실행
    /// </summary>
    /// <returns>hp 가 0 이하라면 ture 아니면 false 를 반환</returns>
    public bool CheckLiving()
    {
        if (HP <= 0)
        {
            FindObjectOfType<SoundManager>().Play("Player/PlayerDead",SoundType.Effect);
            PlayerAnim.SetBool("Dead", true);
            Playerstate = PlayerstateEnum.Dead;
            StartCoroutine(ReviveCo());
            return true;
        }
        else
            return false;
    }
    private IEnumerator ReviveCo()
    {
        //todo: 나중에는 죽었던 시점의 전 방으로 부활
        GameObject.Find("Canvas").transform.Find("FadeImage").GetComponent<FadeImage>().FadeIn();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Dungeon");
        
    }
    
    /// <summary>
    /// 플레이어 이동 방식 버전 1
    /// rigidbody 속 movePosition 사용
    /// </summary>
    private void PlayerMove_v1()
    {
        
        float h = Input.GetAxisRaw("Horizontal");//x축 으로 이동할때 사용할 변수, 받을 입력값 : a,d
        float v = Input.GetAxisRaw("Vertical");//z 축으로 이동할때 사용할 변수, 받을 입력값 : w,s
                                               //플레이어가 가는 방향으로 보도록 sprite 돌려주기
        if (Input.GetButton("Horizontal") && !_isWallslide && Playerstate != PlayerstateEnum.Attack)
        {
            this.transform.GetChild(0).localScale = new Vector3(Input.GetAxisRaw("Horizontal") == -1 ? -2.5f : 2.5f, 2.5f, 1);
        }


        //방향키가 눌렸을때에는 달리는 상태로 아니면 idel 상태로 둔다
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            //달리는 상태로 변환
            PlayerAnim.SetBool("IsRun", true);
            //플레이어가 공격중이 아닐때만 이동할 수 있도록 설정
            if (Playerstate != PlayerstateEnum.Attack)
            {

             //   Vector3 movement = new Vector3(h, 0, v * 0.5f) * Time.deltaTime * Speed;
                
                _rigid.velocity = this._rigid.velocity.y * Vector3.up;

                if (_isWall)
                {
                   
                    if (_isWallslide) { 
                        //오른쪽
                        if (this.transform.GetChild(0).localScale.x > 0)
                        {
                            h = Mathf.Clamp(h, 0, 1);
                        }
                        else
                        {
                            h = Mathf.Clamp(h, -1, 0);
                        }
                    }
                    else
                    {
                        if (this.transform.GetChild(0).localScale.x > 0)
                        {
                            h = Mathf.Clamp(h, -1, 0);
                        }
                        else
                        {
                            h = Mathf.Clamp(h, 0, 1);
                        }
                    }
                }

                if (_isStair)
                {
                    transform.position += Vector3.up * 0.1f;
                    transform.position += Vector3.right * h * 0.05f;
                    //_isStair = false;
                }
                 
                _rigid.velocity += Vector3.right * h   * Speed;
                
                _rigid.velocity += Vector3.forward * v * 0.5f  * Speed;
                //  _rigid.AddForce(new Vector3(h, 0, v * 0.5f) * Time.deltaTime * Speed, ForceMode.VelocityChange);
                //_rigid.velocity = new Vector3(h, 0, v * 0.5f) * Time.deltaTime * Speed;
                // _rigid.MovePosition(this.transform.position + movement);
            }
        }
        else
        {
            //플레이어가 idel 로 변경
            PlayerAnim.SetBool("IsRun", false);
            
            if(!_isFry)
            _rigid.velocity =  new Vector3(_rigid.velocity.x * 0.75f, _rigid.velocity.y * 1f, _rigid.velocity.z * 0.75f);
        }



    }


    public void ChangeLadder(GameObject colliderObj, bool changeLadder)
    {
        _isLadder = changeLadder;
        //사다리 탔을 때
        if (_isLadder)
        {
            if (colliderObj.transform.Find("Collider").transform.Find("Top"))
            {
                GameObject.Find("Colliders").transform.Find("Collider_Dungeon2").transform.Find("UnderCollider").gameObject.SetActive(false);
                Debug.Log("위쪽에서");
                ClimbLadder(colliderObj,-0.5f);
            }
            else if (colliderObj.transform.Find("Collider").transform.Find("Bottom"))
            {
                Debug.Log("아래쪽에서");
                ClimbLadder(colliderObj, 1f);
            }
        }
        else//사다리에서 내릴 때 순간이동하여 다른 바닥에 착지
        {
            Ray ray = new Ray(this.transform.position, Vector3.forward);

            if (Physics.Raycast(ray, 1f, LayerMask.GetMask("Wall")))
            {
                Debug.Log("벽일때");
                this.transform.position = new Vector3(colliderObj.transform.position.x, this.transform.position.y,
                    colliderObj.transform.position.z - 1f);
            }
            else//위쪽에서 내릴 때
            {
                GameObject.Find("Colliders").transform.Find("Collider_Dungeon2").transform.Find("UnderCollider").gameObject.SetActive(true);
                this.transform.position = new Vector3(colliderObj.transform.position.x - 1f, this.transform.position.y + 0.5f,
                    colliderObj.transform.position.z);
            }
            GetComponent<Rigidbody>().useGravity = true;
            PlayerAnim.SetBool("Ladder", false);
        }
    }

    /// <summary>
    /// 사다리에 상호작용을 했을 때
    /// </summary>
    /// <param name="startPos">사다리 탑승 시 y축 시작부분</param>
    private void ClimbLadder(GameObject ladder, float startPos)
    {
        GetComponent<Rigidbody>().useGravity = false;
        this.transform.position = new Vector3(ladder.transform.position.x, this.transform.position.y + startPos,
            ladder.transform.position.z - 0.1f);

        PlayerAnim.SetBool("Ladder", true);
        PlayerAnim.SetTrigger("LadderTri");
    }

    private void LadderMove()
    {
        float v = Input.GetAxisRaw("Vertical") * 2;

        if (v != 0)
            PlayerAnim.speed = 1;
        
        this.transform.Translate(Vector3.up * v * Time.deltaTime);
    }


    private void PlayerJump()
    {
        //대화중이 아닐때만 점프
        if (Playerstate != PlayerstateEnum.Attack)
        {

            //플레이어가 공중에 있는지 확인하여 공중에 떠있지 않을때만 점프를 할 수 있도록 설정

            if (Input.GetKey(KeyCode.C))
            {
                if (!_isFry)
                {
                    //플레이어가 y 축으로 올라갈 수 있도록 velocity 를 재설정
                    gameObject.GetComponent<Rigidbody>().velocity =
                        new Vector3(_rigid.velocity.x, 1 * 10f, _rigid.velocity.z);
                    
                   
                }
                else
                {

                    //벽에 슬라이드중
                    if (_isWallslide)
                    {
                                gameObject.GetComponent<Rigidbody>().velocity =
                            new Vector3(this.transform.GetChild(0).localScale.x < 0 ? -7f : 7f, 8f, _rigid.velocity.z);
                            //점프 애니메이션
                        
                            _isWallslide = false;
                            PlayerAnim.SetBool("IsJump", true);
                            PlayerAnim.SetBool("WallSlide", false);
                            Physics.gravity = Vector3.down * 25f;
                            
                        
                        
                    }
                }

            }

        }
    }



    /// <summary>
    /// 플레이어가 하늘을 날고 있는지 확인하는 하여 _isFry 변수를 변경하는 함수
    /// </summary>
    private void CheckFry()
    {
        RaycastHit hit;
        //자신 기준 플레이어 sprite y 축 크기의 절반만큼 빼서 플레이어 발 에서 부터 ray 를 출력할 수 있도록 좌표설정


        Ray ray = new Ray(transform.position + new Vector3(_playerSprite.sprite.rect.width/ _playerSprite.sprite.pixelsPerUnit * this.transform.localScale.x 
            * Input.GetAxisRaw("Horizontal")
            , -(_playerSprite.sprite.rect.height / _playerSprite.sprite.pixelsPerUnit) * this.transform.localScale.y, 0),
            Vector3.down);
        //아래 방향을로 ray 를 발사하여 Floor layer 만 충돌하도록 설정
        //Debug.Log(_playerSprite.sprite.rect.height / _playerSprite.sprite.pixelsPerUnit * this.transform.localScale.y);
        LayerMask layerMask = LayerMask.GetMask("Floor", "Wall", "InterationObj");

        if(Physics.Raycast(ray,out hit, LayerMask.GetMask("Stair")))
        {
            //바닥과 플레이어 사이의 거리
            float Distance = hit.distance;
            //  Debug.Log(Distance);
            //바닥과의 거리가 1f 이상 떨어지고 플레이어의 힘이 위쪽을 향하고 있다면
            if (!_isFry && Distance > 0.1f && _rigid.velocity.y > 3f)
            {
                ChangeFry(true);
                PlayerAnim.SetBool("IsJump", true);
                PlayerAnim.SetBool("IsRun", false);
            }
            //플레이어가 날고 있고 플레이어의 힘이 아래쪽으로 떨어지고 있다면
            if (_isFry && _rigid.velocity.y < 9.8f)
            {//낙하 애니메이션
                PlayerAnim.SetBool("IsFall", true);
            }
            //플레이어가 땅에 도착할때
            if (_isFry && Distance < 0.1f)
            {
                PlayerAnim.SetBool("IsFall", false);
                PlayerAnim.SetBool("IsJump", false);

                ChangeFry(false);
                _isWall = false;
                _wallslideObject = this.gameObject.GetInstanceID();
                if (_isWallslide)
                {

                    _isWallslide = false;
                    PlayerAnim.SetBool("WallSlide", false);
                    Physics.gravity = Vector3.down * 25f;
                }
            }
        
        }else if (Physics.Raycast(ray, out hit, layerMask))
        {
            //바닥과 플레이어 사이의 거리
            float Distance = hit.distance;
          //  Debug.Log(Distance);
            //바닥과의 거리가 1f 이상 떨어지고 플레이어의 힘이 위쪽을 향하고 있다면
            if (!_isFry && Distance > 0.1f)
            {
                ChangeFry(true);
                PlayerAnim.SetBool("IsJump", true);
            }
            //플레이어가 날고 있고 플레이어의 힘이 아래쪽으로 떨어지고 있다면
            if (_isFry && _rigid.velocity.y < 9.8f)
            {//낙하 애니메이션
                PlayerAnim.SetBool("IsFall", true);
            }
            //플레이어가 땅에 도착할때
            if (_isFry && Distance < 0.1f)
            {
               
                PlayerAnim.SetBool("IsFall", false);
                PlayerAnim.SetBool("IsJump", false);

                ChangeFry(false);
                _isWall = false;
                _wallslideObject = this.gameObject.GetInstanceID();
                if (_isWallslide)
                {
                    
                    _isWallslide = false;
                    PlayerAnim.SetBool("WallSlide", false);
                    Physics.gravity = Vector3.down * 25f;
                }
            }
        }
    }

    public void PlayerAttack()
    {

        if(CountTimeList[1] <= 0) {

            if (Playerstate == PlayerstateEnum.Idle)
            {
                if (Speed != _oriSpeed)
                {
                    Speed = _oriSpeed;
                }
            }
            if (!_isFry) { 
            if (Input.GetKeyDown(KeyCode.Z))
            {
                PlayerAnim.SetTrigger("AgainAttack");
                CountTimeList[1] = 0.98f;
                
                StartCoroutine(Zattackmove());

                  _isAgainAttack = true;
            }
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                PlayerAnim.SetTrigger("Attack");
                CountTimeList[1] = 0.34f;
                Speed = _slowSpeed;
               // Playerstate = PlayerstateEnum.Attack;
                _isAgainAttack = true;
            }
           
        }
    }

    IEnumerator Zattackmove()
    {
        Vector3 vector;
        if(this.transform.GetChild(0).localScale.x < 0)
        {
            vector = Vector3.left;
        }
        else
        {
            vector = Vector3.right;
        }
        yield return new WaitForSeconds(0.15f);

        for(int i = 0; i < 16; i++)
        {

            this.transform.Translate(vector * Time.deltaTime * Speed * 0.75f);
            yield return new WaitForEndOfFrame();

        }
    }

    public void AllstopSkillCor()
    {
      //  StopAllCoroutines();
        for(int i = 0; i < enumerators.Length
            ; i++)
        {
            if(enumerators[i] != null)
            StopCoroutine(enumerators[i]);
        }
    }

    public void Skill()
    {

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (CountTimeList[2] <= 0)
            {
                CountTimeList[0] += 1.5f;
                CountTimeList[2] = 10f;
                AllstopSkillCor();
                PlayerAnim.SetTrigger("Skill1");
                Debug.Log("!!");
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (CountTimeList[3] <= 0)
            {
                CountTimeList[3] = 4f;
                AllstopSkillCor();
                enumerators[1] = SkillTwoCor();
                StartCoroutine(enumerators[1]);
                Playerstate = PlayerstateEnum.Skill;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (CountTimeList[4] <= 0)
            {
                CountTimeList[0] += 1.5f;
                CountTimeList[4] = 12f;
                AllstopSkillCor();
                PlayerAnim.SetTrigger("Skill3");
             
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
           
                AllstopSkillCor();
                Roll();
            
        }


    }

    public void SkillOne()
    {
        enumerators[0] = SkillOneCor();
        StartCoroutine(enumerators[0]);
    }

    IEnumerator SkillOneCor()
    {
        Vector3 dic = this.transform.GetChild(0).localScale.x < 0 ? Vector3.left : Vector3.right;
        float distance = this.transform.GetChild(0).localScale.x < 0 ? -3f : 3f;

        Ray ray = new Ray(this.transform.position, dic);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 4f, LayerMask.GetMask("Wall")))
        {
            distance = hit.distance * 0.8f;
        }

        Vector3 startpos = this.transform.position;
        Vector3 endpos = startpos + (Vector3.right * distance);
        
        Playerstate = PlayerstateEnum.ncSkill;
        yield return new WaitForSeconds(PlayerAnim.GetCurrentAnimatorStateInfo(0).length * 0.1f);
        
      
        _rigid.velocity = Vector3.zero;
        for (int i = 1; i <= 16; i++)
        {
            Playerstate = PlayerstateEnum.ncSkill;
            this.transform.position = Vector3.Slerp(startpos, endpos, i / 16);
            yield return new WaitForEndOfFrame();
        }
        _playerEffectAnim.SetTrigger("Skill1");


       /* yield return new WaitForSeconds(PlayerAnim.GetCurrentAnimatorStateInfo(0).length 
            - PlayerAnim.GetCurrentAnimatorStateInfo(0).length * (PlayerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime+0.03f));
       
        Playerstate = PlayerstateEnum.Idle;*/
    }

    IEnumerator SkillTwoCor()
    {
        for (int i = 0; i < 3; ++i)
        {
            PlayerAnim.SetTrigger("Skill2");
            _playerEffectAnim.SetTrigger("Skill2");
            yield return new WaitForSecondsRealtime(0.31f);
        }
        FindObjectOfType<SoundManager>().Play("Player/BulletDrop", SoundType.Effect);
        yield return new WaitForSecondsRealtime(0.09f);
        Playerstate = PlayerstateEnum.Idle;//스킬이 끝나는 타이밍
    }


    public void SkillTwo()
    {
        int index = FindBullet();
        BulletPool[index].SetActive(true);
        BulletPool[index].transform.position = this.transform.position;
        if (this.transform.GetChild(0).localScale.x < 0)
        {
            BulletPool[index].transform.rotation = new Quaternion(0, 180, 0, 0);
        }
        else
        {
            BulletPool[index].transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        BulletPool[index].GetComponent<Bullet>().Power = Power * 1;//계수 추가
        BulletPool[index].GetComponent<Bullet>().Speed = 5;

    }


    public int FindBullet()
    {
        int index = 0;
        for (int i = 0; i < BulletPool.Length; i++)
        {
            if (!BulletPool[i].activeSelf)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public void SkillThree(List<GameObject> hitObj)
    {

        //이 함수를 호출하는 타이밍이 공격을 시작하고 나서이다. 그전에 공격할지 말지를 정해야 할것
        Playerstate = PlayerstateEnum.ncSkill;
       // PlayerAnim.SetTrigger("Skill3");
       
        if(hitObj.Count == 0)
        {
            PlayerAnim.SetTrigger("NotFlyattack");
            Playerstate = PlayerstateEnum.Idle;
        }else if (hitObj.Count == 1 && hitObj[0].gameObject.name.Contains("Demon")) {
            _playerEffectAnim.SetTrigger("Skill3");
        }else if(hitObj.Count > 0) { 
        CountTimeList[0] += 1f;
        enumerators[2] = SkillThreeCor(hitObj);
        StartCoroutine(enumerators[2]);
        _playerEffectAnim.SetTrigger("Skill3");
        }
        
    }

    IEnumerator SkillThreeCor(List<GameObject> hitObj)
    {
        GameObject[] gameObjects = new GameObject[hitObj.Count];
        _rigid.velocity = Vector3.zero;

        Debug.LogFormat("hitobj : {0}", hitObj.Count);
        for(int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i] = hitObj[i];
        }

       
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i].name.Contains("Demon"))continue;
            gameObjects[i].GetComponent<I_EnemyControl>()._enemystate = Enemystate.Stop;
            gameObjects[i].GetComponent<NavMeshAgent>().enabled = false;   
            gameObjects[i].GetComponentInChildren<Animator>().SetTrigger("Hitstart");
        }
        

        for (int timeline = 0; timeline < 10; timeline++)
        {
            
            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (gameObjects[i].name.Contains("Demon")) continue;
                gameObjects[i].transform.position += Vector3.up * 0.1f;
            }
            
            yield return new WaitForEndOfFrame();

        }

        yield return new WaitForSeconds(0.02f);
        if (gameObjects.Length >= 1)
        {
            ChangeFry(true);
            PlayerAnim.SetBool("IsJump", true);
            for (int timeline = 0; timeline < 10; timeline++)
            {


                _rigid.useGravity = false;
                this.transform.position += Vector3.up * 0.1f;


                yield return new WaitForEndOfFrame();

            }
        }
       
        
        
        //yield return new WaitForSeconds(0.05f);

        yield return new WaitForSeconds(1f);
        Debug.LogFormat("hitobj : {0}", hitObj.Count);
        Debug.Log("wait");
        if (gameObjects.Length >= 1)
        {
            _rigid.useGravity = true;


        }

        for (int timeline = 0; timeline < 10; timeline++)
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (gameObjects[i].name.Contains("Demon")) continue;
                gameObjects[i].transform.position += Vector3.down * 0.1f;
            }
           
            
        }

        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i].name.Contains("Demon")) continue;
            gameObjects[i].GetComponent<I_EnemyControl>()._enemystate = Enemystate.Find;
            gameObjects[i].GetComponent<NavMeshAgent>().enabled = true;
            gameObjects[i].GetComponentInChildren<Animator>().SetTrigger("Hitstop");

        }
        PlayerAnim.SetBool("IsFail", true);
        Playerstate = PlayerstateEnum.Idle;
        
        yield return null;
        
    }

    public void Roll()
    {
        PlayerAnim.SetTrigger("Roll");
        StartCoroutine(RollCor());
    }

    IEnumerator RollCor()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dic = Vector3.zero;
        if (Mathf.Abs(h) > 0.1f) { 
         dic = new Vector3(h, 0, v * 0.5f).normalized;
        }
        else
        {
            if(this.transform.GetChild(0).transform.localScale.x < 0)
            {
                dic = Vector3.left;
            }
            else
            {
                dic = Vector3.right;
            }
        }
        float Distance = 9f;
        Playerstate = PlayerstateEnum.Skill;
        _isRoll = true;
        CountTimeList[0] = 3f;
        for (int i = 0; i < 13; ++i)
        {
            this.transform.Translate(dic * Distance * Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }
        Playerstate = PlayerstateEnum.Idle;
        _isRoll = false;
        CountTimeList[0] = 0.01f;
    }

    /// <summary>
    /// 플레이어 UI 업데이트 할 때 사용할 예정이 함수(데이터 전달 및 갱신)
    /// </summary>
    private void UpdateUI()
    {
        if (PlayerUIObj != null)
        {
            PlayerUIObj.GetChild(0).GetComponent<Text>().text = "체력:" + HP.ToString();
            PlayerUIObj.GetChild(1).GetComponent<Text>().text = "무적시간:" + CountTimeList[0].ToString();
        }
    }

    /// <summary>
    /// _isFry를 1번째 매개변수로 변경하는 함수
    /// </summary>
    /// <param name="p_Fry">변경한 매개변수 값</param>
    public void ChangeFry(bool p_Fry)
    {
        _isFry = p_Fry;
    }


   

    private void WallSlide()
    {
        Ray ray;
        if (this.transform.GetChild(0).localScale.x < 0)
        {
            ray = new Ray(this.transform.position + Vector3.down * (_playerSprite.sprite.rect.height / _playerSprite.sprite.pixelsPerUnit) * this.transform.localScale.y
                , Vector3.left);
        }
        else
        {
            ray = new Ray(this.transform.position + Vector3.down * (_playerSprite.sprite.rect.height / _playerSprite.sprite.pixelsPerUnit) * this.transform.localScale.y
                , Vector3.right);
        }

        WallSlideRaycast(ray);
    }

    private void WallSlideRaycast(Ray ray)
    {

        float Distance = (_playerSprite.sprite.rect.width * 0.48f) / _playerSprite.sprite.pixelsPerUnit * this.transform.localScale.x;
        
        if (Physics.Raycast(ray, out _wallslidehit, Distance, LayerMask.GetMask("Wall")))
        {
            _isWall = true;
            Debug.Log("1벽충돌");
            if(_wallslidehit.transform.gameObject.GetInstanceID() != _wallslideObject) { 
                if (!_isWallslide)
                {
                    Debug.Log("2벽충돌");
                    FindObjectOfType<SoundManager>().Play("Player/PlayerWall",SoundType.Effect);
                    Physics.gravity = Vector3.down * 5f;
                    _rigid.velocity = Vector3.zero;
                    this.transform.GetChild(0).localScale = new Vector3(this.transform.GetChild(0).localScale.x * -1,
                    this.transform.GetChild(0).localScale.y,
                    this.transform.GetChild(0).localScale.z);
                    PlayerAnim.SetBool("WallSlide", true);
                    _isWallslide = true;
                    _wallslideObject = _wallslidehit.transform.gameObject.GetInstanceID();
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            _isWall = false;
            if (_isWallslide)
            {
            
                Debug.Log("벽떨어짐");
                _isWallslide = false;
                PlayerAnim.SetBool("WallSlide", false);
                Physics.gravity = Vector3.down * 25;
                _wallslideObject = this.gameObject.GetInstanceID();
            }
        }
    }

     private void OnCollisionEnter(Collision collision)
      {

        if (_isFry) 
            WallSlide();
        

      }
     
     /// <summary>
     /// 플레이어의 방향을 바꿔주는 함수
     /// </summary>
     /// <param name="scaleX"></param>
     public void ChangeDirection(bool isChange = true)
     {
         if(isChange)
             this.transform.GetChild(0).localScale = new Vector3(2.5f, 2.5f, 1);
         else if(!isChange)
             this.transform.GetChild(0).localScale = new Vector3(-2.5f, 2.5f, 1);
     }
}
