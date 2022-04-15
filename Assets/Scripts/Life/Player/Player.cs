using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    public enum PlayerstateEnum { Idle, Attack, Skill, Dead }
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

    private bool _isRoll = false;

    public bool _isLadder = false;

    private float _slowSpeed;
    private float _oriSpeed;


    public void Awake()
    {
        //필요한 컴포넌트, 데이터들을 초기화 해준다.
        _playerAnim = transform.GetChild(0).GetComponent<Animator>();
        _playerEffectAnim = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        _playerSprite = GetComponentInChildren<SpriteRenderer>();
        _rigid = GetComponent<Rigidbody>();
        //스텟을 초기화 해주는 함수.
        Initdata(30, 10, 3);
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
    }


    private void Update()
    {
        countTime();
        CheckFry();
        if (!_isLadder && !_isWallslide && !SceneManager.GetActiveScene().name.Equals("Town"))
        {
            if (!IsStop && (Playerstate != PlayerstateEnum.Dead && Playerstate != PlayerstateEnum.Skill))
            {
                PlayerAttack();

            }
            if (!IsStop && Playerstate != PlayerstateEnum.Dead && !_isRoll)
            {
                Skill();
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateUI();
        if (!_isLadder)
        {
            if (!SceneManager.GetActiveScene().name.Equals("Town") && !IsStop && (Playerstate != PlayerstateEnum.Dead
                    && Playerstate != PlayerstateEnum.Skill))
            {
                PlayerJump();
                WallSlide();
            }

            if (!IsStop)
            {
                PlayerMove_v1();
            }
        }
        else
        {
            //사다리 타고 있을때
            LadderMove();
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
    public bool Gethit(int Cvalue)
    {
        //데미지가 들어오니 무적 카운트와 hit 애니메이션 실행
        if (Cvalue > 0)
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

        if (HP <= 0)
        {
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
        if (Input.GetButton("Horizontal") && !_isWallslide && Playerstate != PlayerstateEnum.Attack)
        {
            this.transform.GetChild(0).localScale = new Vector3(Input.GetAxisRaw("Horizontal") == -1 ? -2.5f : 2.5f, 2.5f, 1);
        }


        //방향키가 눌렸을때에는 달리는 상태로 아니면 idel 상태로 둔다
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            //달리는 상태로 변환
            _playerAnim.SetBool("IsRun", true);
            //플레이어가 공격중이 아닐때만 이동할 수 있도록 설정
            if (Playerstate != PlayerstateEnum.Attack)
            {

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


    public void ChangeLadder(GameObject colliderObj, bool changeLadder)
    {
        _isLadder = changeLadder;
        if (_isLadder)
        {
            GetComponent<Rigidbody>().useGravity = false;
            this.transform.position = new Vector3(colliderObj.transform.position.x, this.transform.position.y, colliderObj.transform.position.z - 0.25f);

            _playerAnim.SetBool("Ladder", true);
            _playerAnim.SetTrigger("LadderTri");
        }
        else
        {
            Ray ray = new Ray(this.transform.position, Vector3.forward);

            if (Physics.Raycast(ray, 1f, LayerMask.GetMask("Wall")))
            {
                this.transform.position = new Vector3(colliderObj.transform.position.x, this.transform.position.y, colliderObj.transform.position.z - 0.25f);
            }
            else
            {
                this.transform.position = new Vector3(colliderObj.transform.position.x, this.transform.position.y + 0.4f, colliderObj.transform.position.z + 0.5f);
            }
            GetComponent<Rigidbody>().useGravity = true;
            _playerAnim.SetBool("Ladder", false);
        }
    }

    private void LadderMove()
    {
        float v = Input.GetAxisRaw("Vertical");

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
                    //점프 애니메이션
                    _playerAnim.SetBool("IsJump", true);
                }
                else
                {
                    //벽에 슬라이드중
                    if (_isWallslide)
                    {
                        gameObject.GetComponent<Rigidbody>().velocity =
                      new Vector3(this.transform.GetChild(0).localScale.x < 0 ? -5f : 5f, 1 * 10f, _rigid.velocity.z);
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
        Ray ray = new Ray(transform.position - new Vector3(0, _playerSprite.sprite.rect.height / _playerSprite.sprite.pixelsPerUnit, 0), Vector3.down);
        //아래 방향을로 ray 를 발사하여 Floor layer 만 충돌하도록 설정
        Debug.Log(_playerSprite.sprite.rect.height / _playerSprite.sprite.pixelsPerUnit);
        LayerMask layerMask = LayerMask.GetMask("Floor", "Wall", "InterationObj");

        if (Physics.Raycast(ray, out hit, layerMask))
        {
            //바닥과 플레이어 사이의 거리
            float Distance = hit.distance;
            Debug.Log(Distance);
            //바닥과의 거리가 1f 이상 떨어지고 플레이어의 힘이 위쪽을 향하고 있다면
            if (!_isFry && Distance > 0.1f)
            {
                ChangeFry(true);

            }
            //플레이어가 날고 있고 플레이어의 힘이 아래쪽으로 떨어지고 있다면
            if (_isFry && _rigid.velocity.y < 0)
            {//낙하 애니메이션
                _playerAnim.SetBool("IsFall", true);
            }
            //플레이어가 땅에 도착할때
            if (_isFry && Distance < 0.1f)
            {
                _playerAnim.SetBool("IsFall", false);
                _playerAnim.SetBool("IsJump", false);

                ChangeFry(false);
                if (_isWallslide)
                {
                    _isWallslide = false;
                    _playerAnim.SetTrigger("WallSlideout");
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

            if (Input.GetKeyDown(KeyCode.Z))
            {
                _playerAnim.SetTrigger("AgainAttack");
                CountTimeList[1] = 0.77f;
                Playerstate = PlayerstateEnum.Attack;
                StartCoroutine(Zattackmove());

                  _isAgainAttack = true;
            }


            if (Input.GetKeyDown(KeyCode.X))
            {
                _playerAnim.SetTrigger("Attack");
                CountTimeList[1] = 0.77f;
                Speed = _slowSpeed;
               // Playerstate = PlayerstateEnum.Attack;
                _isAgainAttack = true;
            }
           
        }
        else
        {
           
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
        StopAllCoroutines();
        /*  StopCoroutine(SkillOneCor());
          StopCoroutine(SkillTwoCor());
          StopCoroutine(RollCor());*/
    }

    public void Skill()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (CountTimeList[2] <= 0)
            {
                CountTimeList[2] = 3f;
                AllstopSkillCor();
                _playerAnim.SetTrigger("Skill1");
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (CountTimeList[3] <= 0)
            {
                CountTimeList[3] = 2f;
                AllstopSkillCor();
                StartCoroutine(SkillTwoCor());
                Playerstate = PlayerstateEnum.Skill;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (CountTimeList[4] <= 0)
            {

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
        StartCoroutine(SkillOneCor());
    }

    IEnumerator SkillOneCor()
    {
        Vector3 dic = this.transform.GetChild(0).localScale.x < 0 ? Vector3.left : Vector3.right;
        float distance = this.transform.GetChild(0).localScale.x < 0 ? -3f : 3f;

        Ray ray = new Ray(this.transform.position, dic);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 4f, LayerMask.GetMask("Wall")))
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

        while (_playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }

        Playerstate = PlayerstateEnum.Idle;
    }

    IEnumerator SkillTwoCor()
    {
        for (int i = 0; i < 3; ++i)
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

    public void Roll()
    {
        _playerAnim.SetTrigger("Roll");
        StartCoroutine(RollCor());
    }

    IEnumerator RollCor()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dic = new Vector3(h, 0, v * 0.5f).normalized;
        float Distance = 10f;
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

        float Distance = (_playerSprite.sprite.rect.width * 0.48f) / _playerSprite.sprite.pixelsPerUnit;

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
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                Debug.Log("벽떨어짐");
                _isWallslide = false;
                _playerAnim.SetTrigger("WallSlideout");
                Physics.gravity = Vector3.down * 25;
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
