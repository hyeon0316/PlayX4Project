using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
1.public으로 선언 된 변수는 앞글자 대문자로 시작
2. private는 변수 앞에 "_"붙이고  소문자로 시작
3. 함수 안의 지역변수는 네이밍 규칙 상관없음
4. bool형 변수는 is~ or can~ 로 시작
번외로  나중에 구현해야 할 일을 스크립트에 적어 놓을때 //todo: 할 일(메모장 느낌)
*/


public class Player : Life,I_hp
{

    private bool _isFry = false;

    private bool _doubleJump;

    private void Awake()
    {
        Initdata(30, 10, 3);
    }


    private void FixedUpdate()
    {
        CheckFry();
        PlayerJump();
        PlayerMove_v1();
    }

   
    public bool Gethit(int Cvalue)
    {
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


      
        Vector3 movement = new Vector3(h,0, v * 0.5f) * Time.deltaTime * Speed;

        //rigidbody 를 이용해서 이동하는 라인.

        //gameObject.GetComponent<Rigidbody>().AddForce(movement, ForceMode.Impulse);
        gameObject.GetComponent<Rigidbody>().MovePosition(this.transform.position + movement);
    }

    private void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!_isFry) { 
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(gameObject.GetComponent<Rigidbody>().velocity.x, 1 * 7f,0);
            
            }else if (_doubleJump)
            {
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(gameObject.GetComponent<Rigidbody>().velocity.x, 1 * 7f, 0);
                _doubleJump = false;
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
          
            if(!_isFry && Distance > 1.2f && gameObject.GetComponent<Rigidbody>().velocity.y > 1f)
            {
                _doubleJump = true;
                ChangeFry(true);
            }

            if(_isFry && Distance < 0.75f)//플레이어가 날고 있을때 floor 가 hit 에 들어갈때
            {
                Debug.Log("Floor충돌");
                    _doubleJump = false;
                    ChangeFry(false);
               
            }
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



}
