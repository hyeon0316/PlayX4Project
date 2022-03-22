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


public class Player : MonoBehaviour
{
    //플레이어의 이동속도를 결정
    public float Speed = 10;
    //v4 에서 velocity 에 값을 입력할때 사용할 최대 속도
    public float MaxSpeed = 5;

    public int Movetype = 1;

    private bool _isFry = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckFry();
        PlayerType();
    }

    /// <summary>
    /// 테스트용 이동 타입 설정
    /// </summary>
    private void PlayerType()
    {
        switch (Movetype)
        {
            case 0:
                PlayerMove_v1();
                break;
            case 1:
                PlayerMove_v2();
                break;
            case 2:
                PlayerMove_v3();
                break;
            case 3:
                PlayerMove_v4();
                break;
        }
    }



    /// <summary>
    /// 플레이어 이동 방식 버전 1
    /// 좌우 이동에 앞뒤로 이동하는 버전, rigidbody 속 movePosition 사용
    /// </summary>
    private void PlayerMove_v1()
    {
        float h = Input.GetAxis("Horizontal");//x축 으로 이동할때 사용할 변수, 받을 입력값 : a,d
        float v = Input.GetAxis("Vertical");//z 축으로 이동할때 사용할 변수, 받을 입력값 : w,s

        Vector3 movement = new Vector3(h, 0, v) * Time.deltaTime * Speed;

        //rigidbody 를 이용해서 이동하는 라인.
        gameObject.GetComponent<Rigidbody>().MovePosition(gameObject.transform.position + movement);

    }


    /// <summary>
    /// 플레이어 이동방식 버전 2
    /// 좌우 이동에 점프가 되는 버전, rigidbody 속 movePosition 사용
    /// </summary>
    private void PlayerMove_v2()
    {
        float h = Input.GetAxis("Horizontal");//x축 으로 이동할때 사용할 변수, 받을 입력값 : a,d
        PlayerJump();
         Vector3 movement = new Vector3(h, 0, 0) * Time.deltaTime * Speed;

        //rigidbody 를 이용해서 이동하는 라인.
        gameObject.GetComponent<Rigidbody>().MovePosition(gameObject.transform.position + movement);

    }
    /// <summary>
    /// 플레이어 이동방식 버전 3
    /// 좌우이동에 점프가 되는 버전, transform.Translaate()사용.
    /// </summary>
    private void PlayerMove_v3()
    {
        float h = Input.GetAxis("Horizontal");
        PlayerJump();


        Vector3 movement = new Vector3(h, 0, 0) * Time.deltaTime * Speed;

        //translate 를 사용해서 플레이어의 좌표를 변경
        gameObject.transform.Translate(movement);
    }

    private void PlayerMove_v4()
    {

        float h = Input.GetAxis("Horizontal");
        PlayerJump();


        Vector3 movement = new Vector3(h, 0, 0) * Time.deltaTime * Speed;

        //rigidbody 에 가속도 값을 더하여 이동하는 라인
        Rigidbody rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.velocity += movement;

        //무한이 가속하는것을 막기 위해 최대속도 이상이 될경우 최대속도로 고정한다.
        if (rigidbody.velocity.x > MaxSpeed)
            rigidbody.velocity = new Vector3(MaxSpeed, 0, 0);
        if (rigidbody.velocity.x < -MaxSpeed)
            rigidbody.velocity = new Vector3(-MaxSpeed, 0, 0);
    }



    private void PlayerJump()
    {
        if (!_isFry && Input.GetKeyDown(KeyCode.W))
        {

            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 500f);
            ChangeFry(true);
        }
    }

    /// <summary>
    /// 플레이어가 하늘을 날고 있는지 확인하는 하여 _isFry 변수를 변경하는 함수
    /// </summary>
    private void CheckFry()
    {

        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);//플레이어 기준으로 아래 방향으로 ray 생성


        if(_isFry&&Physics.Raycast(ray,out hit, LayerMask.GetMask("floor")))//플레이어가 날고 있을때 floor 가 hit 에 들어갈때
        {
         //   Debug.Log(hit.distance);
            if (hit.distance < (gameObject.GetComponent<MeshRenderer>().bounds.size.y/2)*1.1f) ChangeFry(false);//floor 와의 거리가 플레이어 y 축의 절반 이하일때 
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
