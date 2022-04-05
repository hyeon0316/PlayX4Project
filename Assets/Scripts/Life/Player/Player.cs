using System;
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


public class Player : Life,I_hp
{
    public enum PlayerstateEnum { Idle, Attack,Dead}

    public PlayerstateEnum Playerstate;

    private bool _isFry = false;

    public float[] CountTimeList;

    private Animator _playerAnim;

    private SpriteRenderer _playerSprite;//좌우 이동 시 방향 전환에 쓰일 변수

    private Rigidbody _rigid;
    //플레이어관련된 UI를 담은 부모 오브젝트 ,getchild 를 이용해서 UI를 컨트롤 한다.
    public Transform PlayerUIObj;

    private int _atkNum;
    private float _atkDelay;
    private bool _canAttack = false;
    private bool _isCheck = false;
    private bool _isAgainAttack = false;
    private bool _isDash = false;
    private void Awake()
    {
        _playerAnim = GetComponentInChildren<Animator>();
        _playerSprite = GetComponentInChildren<SpriteRenderer>();
        _rigid = GetComponent<Rigidbody>();
        Initdata(30, 10, 3);
        Playerstate = PlayerstateEnum.Idle;
        CountTimeList = new float[2];
        
    }


    private void Update()
    {
        countTime();
        CheckFry();
        PlayerAttack();
        //방향 전환은 물리기반이 아니기 때문에 Update에서 검사
        if (Input.GetButton("Horizontal"))
        {
            _playerSprite.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        
    }

    private void FixedUpdate()
    {
        UpdateUI();
        PlayerJump();
        PlayerMove_v1();
       
    }

   
    private void countTime()
    {
        for(int i = 0; i < CountTimeList.Length; ++i)
        {
            if(CountTimeList[i] >= 0)
            CountTimeList[i] -= Time.deltaTime;
        }
    }

    public bool Gethit(int Cvalue)
    {
        if(Cvalue > 0)
        {
            //플레이어가 무적상태라면 애니메이션과 채력계산을 무시하고 리턴한다.
            if (CountTimeList[0] > 0)
                return CheckLiving();

            CountTimeList[0] = 1f;
            _playerAnim.SetTrigger("HitPlayer");
        }

        
        HP -= Cvalue;
        return CheckLiving();
    }

    public bool CheckLiving()
    {

        if(HP <=0)
        return true;
        else
        return false;
    }

    /// <summary>
    /// 플레이어 이동 방식 버전 1
    /// 좌우 이동에 앞뒤로 이동하는 버전, rigidbody 속 movePosition 사용
    /// </summary>
    private void PlayerMove_v1()
    {

       
      
            float h = Input.GetAxisRaw("Horizontal");//x축 으로 이동할때 사용할 변수, 받을 입력값 : a,d
            float v = Input.GetAxisRaw("Vertical");//z 축으로 이동할때 사용할 변수, 받을 입력값 : w,s
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                _playerAnim.SetBool("IsRun", true);
                _isDash = true;
                if (Playerstate != PlayerstateEnum.Attack) { 
                    
                    Vector3 movement = new Vector3(h, 0, v * 0.5f) * Time.deltaTime * Speed;
                    _rigid.MovePosition(this.transform.position + movement);
                }
            }
            else
            {
                _isDash = false;
                _playerAnim.SetBool("IsRun", false);
              
            }

            
      
    }

    private void PlayerJump()
    {
        if (Input.GetKey(KeyCode.C))
        {
            if (!_isFry)
            {
                gameObject.GetComponent<Rigidbody>().velocity =
                    new Vector3(_rigid.velocity.x, 1 * 8.5f, 0);
                
                _playerAnim.SetBool("IsJump",true);
            }
        }
    }

    /// <summary>
    /// 플레이어가 하늘을 날고 있는지 확인하는 하여 _isFry 변수를 변경하는 함수
    /// </summary>
    private void CheckFry()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);//플레이어 기준으로 아래 방향으로 ray 생성

        if(Physics.Raycast(ray, out hit, LayerMask.GetMask("Floor"))){
            float Distance = hit.distance;
          
            if(!_isFry && Distance > 1f && _rigid.velocity.y > 0.1f)
            {
                ChangeFry(true);
                _playerAnim.SetBool("IsJump", false);
            }

            if (_isFry && _rigid.velocity.y < -0.1f)
            {
                _playerAnim.SetBool("IsFall", true);
            }

            if(_isFry && Distance < 1f)//플레이어가 날고 있을때 floor 가 hit 에 들어갈때
            {
                _playerAnim.SetBool("IsFall", false);
                Debug.Log("Floor충돌");
                    ChangeFry(false);
            }
        }
    }

    public void PlayerAttack()
    {

        if (_playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle")
            || _playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            Playerstate = PlayerstateEnum.Idle;
        }

        if (_playerAnim.GetCurrentAnimatorStateInfo(0).IsName("AttackIdle")
            || _playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Attackrun")) {

            if (Input.GetKeyUp(KeyCode.X) && !_isCheck)
            {
                Playerstate = PlayerstateEnum.Idle;
                _isAgainAttack = true;
                _isCheck = true;
            }

            if (_isAgainAttack && Input.GetKeyDown(KeyCode.X))
            {
                _playerAnim.SetTrigger("AgainAttack");
                CountTimeList[1] = 3f;
                _isAgainAttack = false;
                
                Playerstate = PlayerstateEnum.Attack;
            }
              

            if ( _playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 3f )
            {
                CountTimeList[1] = 1f;
                _atkNum = 0;
                _playerAnim.SetTrigger("AgainAttackreset");
                _isAgainAttack = false;
                _isCheck = false;
                Playerstate = PlayerstateEnum.Idle;
            }
        }

        if (Input.GetKeyDown(KeyCode.X)&& CountTimeList[1] <= 0)
        {

            _atkNum = 0;
            AttackAnimation(_atkNum);
            CountTimeList[1] = 2f;
            _isCheck = false;
            Playerstate = PlayerstateEnum.Attack;
        }

    }

    private void UpdateUI()
    {
        if(PlayerUIObj!= null) { 
        PlayerUIObj.GetChild(0).GetComponent<Text>().text = "체력:" + HP.ToString();
        PlayerUIObj.GetChild(1).GetComponent<Text>().text = "무적시간:" + CountTimeList[0].ToString();
        }
    }

    /// <summary>
    /// 연속공격 애니메이션
    /// </summary>
    /// <param name="atkNum"></param>
    private void AttackAnimation(int atkNum)
    {
        _playerAnim.SetFloat("Blend", atkNum);
        _playerAnim.SetTrigger("Attack");
        
    }
    
    private void AnimEventcanattack()
    {
        _canAttack = true;
    }

    private void AnimEventendattack()
    {
        _canAttack = false;
    }

    /// <summary>
    /// _isFry를 1번째 매개변수로 변경하는 함수
    /// </summary>
    /// <param name="p_Fry">변경한 매개변수 값</param>
    public void ChangeFry(bool p_Fry)
    {
        _isFry = p_Fry;
    }

   

}
