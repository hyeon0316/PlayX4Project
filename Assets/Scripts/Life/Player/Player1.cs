﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
1.public으로 선언 된 변수는 앞글자 대문자로 시작
2. private는 변수 앞에 "_"붙이고  소문자로 시작
3. 함수 안의 지역변수는 네이밍 규칙 상관없음
4. bool형 변수는 is~ or can~ 로 시작
번외로  나중에 구현해야 할 일을 스크립트에 적어 놓을때 //todo: 할 일(메모장 느낌)
*/


public class Player1 : Life,I_hp
{
    /// <summary>
    /// 플레이어 상태를 알려주는 enum 변수
    /// </summary>
    public enum PlayerstateEnum { Idle, Attack,Skill,Dead}
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
    private Animator _playerAnim;

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
    /// 플레이어가 1번째 공격 이후 2번째 공격을 할 수 있는지 확인한것을 확인할 수 있는 변수
    /// </summary>
    private bool _isCheck = false;
    /// <summary>
    /// 플레이어가 2번째 공격을 할 수 있는지 확인하는 변수
    /// </summary>
    private bool _isAgainAttack = false;
    /// <summary>
    /// 플레이어가 대화하고 있는지 확인하느변수
    /// </summary>
    public bool IsStop = false;
    /// <summary>
    /// 벽타고 있는중인지 확인
    /// </summary>
    private bool _isWallslide = false;

    public void Awake()
    {
        //필요한 컴포넌트, 데이터들을 초기화 해준다.
        _playerAnim = transform.GetChild(0).GetComponent<Animator>();
        _playerEffectAnim = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        _playerSprite = GetComponentInChildren<SpriteRenderer>();
        _rigid = GetComponent<Rigidbody>();
        //스텟을 초기화 해주는 함수.
        Initdata(30, 10, 3);
        Playerstate = PlayerstateEnum.Idle;
        CountTimeList = new float[5];
        BulletParent = GameObject.Find("Bulletpool").transform;
        BulletPool = new GameObject[12];
        for(int i = 0; i < 12; i++) { 
            BulletPool[i] = BulletParent.GetChild(i).gameObject;
            BulletPool[i].SetActive(false);
        }
    }


    private void Update()
    {
        countTime();
        CheckFry();
        if(!IsStop && (Playerstate != PlayerstateEnum.Dead && Playerstate != PlayerstateEnum.Skill)) { 
        PlayerAttack();
        Skill();
        }
    }

    private void FixedUpdate()
    {
        UpdateUI();
        if (!IsStop && (Playerstate != PlayerstateEnum.Dead && Playerstate != PlayerstateEnum.Skill)) { 
        PlayerJump();
        PlayerMove_v1();
        WallSlide();
        }

    }

   /// <summary>
   /// 카운트 해야하는 변수들의 시간을 줄여주는 함수
   /// </summary>
    private void countTime()
    {
        for(int i = 0; i < CountTimeList.Length; ++i)
        {
            if(CountTimeList[i] >= 0)
            CountTimeList[i] -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 플레이어의 체력값을 변경할 수 있는 함수
    /// </summary>
    /// <param name="Cvalue"> 양수가 들어오면 데미지를 입고 음수가 들어오면 회복을 할 수 있다.</param>
    /// <returns>플레이어가 사망한다면 true 아니면 false를 반환한다</returns>
    public bool Gethit(int Cvalue)
    {
        //데미지가 들어오니 무적 카운트와 hit 애니메이션 실행
        if(Cvalue > 0)
        {
           
            //플레이어가 무적상태라면 애니메이션과 채력계산을 무시하고 리턴한다.
            if (CountTimeList[0] > 0)
                return CheckLiving();

            CountTimeList[0] = 1f;
            _playerAnim.SetTrigger("Hit");
        }

        
        HP -= Cvalue;
        return CheckLiving();
    }
    /// <summary>
    /// 플레이어가 살아있는지 확인하는 함수, hp 가 0 이하로 떨어진다면 죽는 애니메이션 실행
    /// </summary>
    /// <returns>hp 가 0 이하라면 ture 아니면 false 를 반환</returns>
    public bool CheckLiving()
    {

        if(HP <= 0) {
            _playerAnim.SetBool("Dead", true);
            Playerstate = PlayerstateEnum.Dead;
        return true;
        }
        else
        return false;
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
            if (Input.GetButton("Horizontal") && !_isWallslide)
            {
            this.transform.GetChild(0).localScale = new Vector3(Input.GetAxisRaw("Horizontal") == -1 ? -2.5f : 2.5f, 2.5f, 1);
            }
           

            //방향키가 눌렸을때에는 달리는 상태로 아니면 idel 상태로 둔다
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                //달리는 상태로 변환
                _playerAnim.SetBool("IsRun", true);
                //플레이어가 공격중이 아닐때만 이동할 수 있도록 설정
                if (Playerstate != PlayerstateEnum.Attack) { 
                    
                    Vector3 movement = new Vector3(h, 0, v * 0.5f) * Time.deltaTime * Speed;
                    _rigid.MovePosition(this.transform.position + movement);
                }
            }
            else
            {
               //플레이어가 idel 로 변경
                _playerAnim.SetBool("IsRun", false);
              
            }

            
      
    }

    private void PlayerJump()
    {
        //대화중이 아닐때만 점프
        if (Playerstate != PlayerstateEnum.Attack) {

            //플레이어가 공중에 있는지 확인하여 공중에 떠있지 않을때만 점프를 할 수 있도록 설정
           
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (!_isFry)
                {
                    //플레이어가 y 축으로 올라갈 수 있도록 velocity 를 재설정
                    gameObject.GetComponent<Rigidbody>().velocity =
                        new Vector3(_rigid.velocity.x, 1 * 10f, _rigid.velocity.z);
                    //점프 애니메이션
                    _playerAnim.SetBool("IsJump", true);
                }
                else
                {
                    //벽에 슬라이드중
                    if (_isWallslide)
                    {
                        gameObject.GetComponent<Rigidbody>().velocity =
                      new Vector3(this.transform.GetChild(0).localScale.x < 0 ? -5f : 5f , 1 * 10f, _rigid.velocity.z);
                        //점프 애니메이션
                        _isWallslide = false;
                        _playerAnim.SetBool("IsJump", true);
                        _playerAnim.SetTrigger("WallSlideout");
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
        Ray ray = new Ray(transform.position - new Vector3(0, _playerSprite.sprite.rect.height/ _playerSprite.sprite.pixelsPerUnit,0), Vector3.down);
        //아래 방향을로 ray 를 발사하여 Floor layer 만 충돌하도록 설정
        Debug.Log(_playerSprite.sprite.rect.height / _playerSprite.sprite.pixelsPerUnit);
        LayerMask layerMask = LayerMask.GetMask("Floor", "Wall", "InterationObj");

        if(Physics.Raycast(ray, out hit, layerMask)){
            //바닥과 플레이어 사이의 거리
            float Distance = hit.distance;
            Debug.Log(Distance);
            //바닥과의 거리가 1f 이상 떨어지고 플레이어의 힘이 위쪽을 향하고 있다면
            if (!_isFry && Distance > 0.1f)
            {
                ChangeFry(true);
                
            }
            //플레이어가 날고 있고 플레이어의 힘이 아래쪽으로 떨어지고 있다면
            if (_isFry && Distance >0.13f && _rigid.velocity.y <0)
            {//낙하 애니메이션
                _playerAnim.SetBool("IsFall", true);
            }
            //플레이어가 땅에 도착할때
            if(_isFry && Distance < 0.08f)
            {
                _playerAnim.SetBool("IsFall", false);
                _playerAnim.SetBool("IsJump", false);
               
                    ChangeFry(false);
                if (_isWallslide) { 
                    _isWallslide = false;
                    _playerAnim.SetTrigger("WallSlideout");
                    Physics.gravity = Vector3.down * 25f;
                }
            }
        }
    }

    public void PlayerAttack()
    {
      

        if (_isAgainAttack)
        {
           
            if (CountTimeList[1] <= 0)
            {
                _isAgainAttack = false;
                Playerstate = PlayerstateEnum.Idle;
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                _playerAnim.SetTrigger("AgainAttack");
                _isAgainAttack = false;
                CountTimeList[1] = 1.7f;
                Playerstate = PlayerstateEnum.Attack;
            }
        }else if(Input.GetKeyDown(KeyCode.X) && CountTimeList[1] <= 0)
        {
            _playerAnim.SetTrigger("Attack");
            CountTimeList[1] = 3f;
            Playerstate = PlayerstateEnum.Attack;
            _isAgainAttack = true;


        }


       /* //플레이어가 공격중이 아닐때에는 idle
        if (_playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle")
            || _playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            Playerstate = PlayerstateEnum.Idle;
        }

        //플레이어가 1번째 애니메이션이 끝나고 2번째 공격전 준비 상태 애니매이션일때
        if (_playerAnim.GetCurrentAnimatorStateInfo(0).IsName("AttackIdle")
            || _playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Attackrun")) {
            //X 키를 올렸을 경우 다음 공격의 플레이어의 상태를 Idel 로 변경하고 2번째 공격의 준비가 되었다고 말한다
            if (Input.GetKeyUp(KeyCode.X) && !_isCheck)
            {
                Playerstate = PlayerstateEnum.Idle;
                _isAgainAttack = true;
                _isCheck = true;
            }
            //공격 준비 상태일때 x 키를 입력하면 2번째 공격을 실행
            if (_isAgainAttack && Input.GetKeyDown(KeyCode.X))
            {
                _playerAnim.SetTrigger("AgainAttack");
                CountTimeList[1] = 1f;//공격 딜레이
                _isAgainAttack = false;
                
                Playerstate = PlayerstateEnum.Attack;
            }
              
            //공격을 하지 않고 애니메이션이 2f 정도 지났을때 일반 idel 로 초기화
            if ( _playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 3f )
            {
                CountTimeList[1] = 1f;
                _playerAnim.SetTrigger("AgainAttackreset");
                _isAgainAttack = false;
                _isCheck = false;
                Playerstate = PlayerstateEnum.Idle;
            }
        }
        //1번째 공격 실행 
        if (Input.GetKeyDown(KeyCode.X)&& CountTimeList[1] <= 0)
        {

            _playerAnim.SetTrigger("Attack");
            CountTimeList[1] = 2f;
            _isCheck = false;
            Playerstate = PlayerstateEnum.Attack;
        }*/

    }

    public void Skill()
    {
       
        switch (Input.inputString)
        {
            case "a":
            case "A":
                if(CountTimeList[2] <= 0) {
                    CountTimeList[2] = 10f;
                _playerAnim.SetTrigger("Skill1");
                }
                break;
            case "s":
            case "S":
                if(CountTimeList[3] <= 0) {
                    CountTimeList[3] = 2f;
                        StartCoroutine(SkillTwoCor());
                Playerstate = PlayerstateEnum.Skill;
                }
                break;
            case "d":
            case "D":
                if(CountTimeList[4] <= 0)
                {

                }
                break;
            case "f":
            case "F":
                    if(Playerstate != PlayerstateEnum.Attack)
                    Roll();
                break;

        }
    }

    public void SkillOne()
    {
        StartCoroutine(SkillOneCor());
    }

    IEnumerator SkillOneCor()
    {
        Vector3 dic = this.transform.GetChild(0).localScale.x < 0 ? Vector3.left : Vector3.right;
        float distance = this.transform.GetChild(0).localScale.x < 0 ? -3f : 3f;

        Ray ray = new Ray(this.transform.position, dic);
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit, 4f, LayerMask.GetMask("Wall")))
        {
            distance = (hit.transform.position.x - this.transform.position.x) * 0.8f;
        }

        Vector3 startpos = this.transform.position;
        Vector3 endpos = startpos + (Vector3.right * distance);
        _playerEffectAnim.SetTrigger("Skill1");
        Playerstate = PlayerstateEnum.Skill;
        for (int i = 1; i <= 6; i++)
        {
            this.transform.position = Vector3.Slerp(startpos, endpos, i / 6);
            yield return new WaitForEndOfFrame();
        }

       while(_playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }

        Playerstate = PlayerstateEnum.Idle;
    }

    IEnumerator SkillTwoCor()
    {
        for(int i = 0; i < 3; ++i)
        {
            _playerAnim.SetTrigger("Skill2");
            _playerEffectAnim.SetTrigger("Skill2");
            yield return new WaitForSecondsRealtime(0.31f);
        }
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
        BulletPool[index].GetComponent<Bullet>().Power = Power;
        BulletPool[index].GetComponent<Bullet>().Speed = 5;
        
    }

   
    public int FindBullet()
    {
        int index = 0;
        for(int i = 0; i < BulletPool.Length; i++)
        {
            if (!BulletPool[i].activeSelf)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public void Roll()
    {
        _playerAnim.SetTrigger("Roll");
        StartCoroutine(RollCor());
    }

    IEnumerator RollCor()
    {
        Vector3 dic = this.transform.GetChild(0).localScale.x < 0 ? Vector3.left : Vector3.right;
        float Distance = 10f; 
        Playerstate = PlayerstateEnum.Skill;
        CountTimeList[0] = 3f;
       for(int i = 0; i < 13; ++i) { 
            this.transform.Translate(dic* Distance * Time.deltaTime);
          
            yield return new WaitForFixedUpdate();
        }
        Playerstate = PlayerstateEnum.Idle;
        CountTimeList[0] = 0.01f;
    } 

    /// <summary>
    /// 플레이어 UI 업데이트 할 때 사용할 예정이 함수(데이터 전달 및 갱신)
    /// </summary>
    private void UpdateUI()
    {
        if(PlayerUIObj!= null) { 
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


    private bool ContactWall(Vector3 contactPoints)
    {
        
       
           if(contactPoints.y - this.transform.position.y > _playerSprite.sprite.rect.width -30/ _playerSprite.sprite.pixelsPerUnit)
           {
                return false;
           }
        
        return true;
    }

    private void WallSlide()
    {
        Ray ray;
        if (this.transform.GetChild(0).localScale.x < 0) {
            ray = new Ray(this.transform.position, Vector3.left);
        }
        else
        {
            ray = new Ray(this.transform.position, Vector3.right);
        }

        WallSlideRaycast(ray);
    }

    private void WallSlideRaycast(Ray ray)
    {
        
        float Distance = (_playerSprite.sprite.rect.width * 0.5f)/ _playerSprite.sprite.pixelsPerUnit;
       
        if (Physics.Raycast(ray, Distance, LayerMask.GetMask("Wall")))
        {
            Debug.Log("1벽충돌");
            if (_isFry && !_isWallslide)
            {
                Debug.Log("2벽충돌");
                Physics.gravity = Vector3.down * 5f;
                _rigid.velocity = Vector3.zero;
                this.transform.GetChild(0).localScale = new Vector3(this.transform.GetChild(0).localScale.x * -1,
                this.transform.GetChild(0).localScale.y,
                this.transform.GetChild(0).localScale.z);
                _playerAnim.SetTrigger("WallSlide");
                _isWallslide = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_isWallslide)
        {
            if(collision.gameObject.layer == LayerMask.GetMask("Wall"))
            {
                _isWallslide = false;
                _playerAnim.SetTrigger("WallSlideout");
                Physics.gravity = Vector3.down * -25;
            }
        }
    }

    /*  private void OnCollisionEnter(Collision collision)
      {

          if(Mathf.Abs(Input.GetAxis("Vertical")) < 0.1f) { 
              if (collision.gameObject.layer == LayerMask.NameToLayer("Wall")){
                  for(int i = 0; i < collision.contactCount; i++)
                  {
                      Debug.LogFormat(" Collision  :{0}", collision.contacts[i].point);
                  }

                  if (ContactWall(collision.transform.position)) { 
                      if (_isFry && !_isWallslide)
                      {

                          Physics.gravity = Vector3.down * 5f;
                          _rigid.velocity = Vector3.zero;
                          this.transform.GetChild(0).localScale = new Vector3(this.transform.GetChild(0).localScale.x * -1,
                          this.transform.GetChild(0).localScale.y,
                          this.transform.GetChild(0).localScale.z);
                           _playerAnim.SetTrigger("WallSlide");
                          _isWallslide = true;
                      }
                  }
              }
          }

      }*/
}
