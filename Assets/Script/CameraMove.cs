using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    /*
1.public으로 선언 된 변수는 앞글자 대문자로 시작
2. private는 변수 앞에 "_"붙이고  소문자로 시작
3. 함수 안의 지역변수는 네이밍 규칙 상관없음
4. bool형 변수는 is~ or can~ 로 시작
번외로  나중에 구현해야 할 일을 스크립트에 적어 놓을때 //todo: 할 일(메모장 느낌)
*/
    //카메라 음직임 타입 테스트가 끝나면 사용하는 함수와 함께 주석
    public int CameraMovetype = 0;
    //플레이어 게임 오브젝트
    public GameObject Player;
    //백그라운드 오브젝트
    public GameObject BackgroundImg;
    //v2를 사용할 경우 카메라가 플레이어를 느리게 따라가는데 그 따라가는 속도롤 결정(0.1~ 0.04 사이로 생각)
    public float CameraMoveSpeed;

    //카메라 크기
    private Vector2 _cameraSize;
    //백그라운드의 최대값
    private Vector2 _maxsize;
    //백그라운드의 최소값
    private Vector2 _minsize;

    // Start is called before the first frame update
    public void Start()
    {
      /*
        orthographic 카메라와 perspective 카메라를 분리하여 2d 카메라와 3d 카메라 를 분리했다.
      */
        _cameraSize.y = transform.GetChild(1).GetComponent<Camera>().orthographicSize;
        _cameraSize.x = _cameraSize.y * transform.GetChild(1).GetComponent<Camera>().aspect;

       

        _maxsize = new Vector2(BackgroundImg.GetComponent<MeshRenderer>().bounds.max.x, BackgroundImg.GetComponent<MeshRenderer>().bounds.max.y);
        _minsize = new Vector2(BackgroundImg.GetComponent<MeshRenderer>().bounds.min.x, BackgroundImg.GetComponent<MeshRenderer>().bounds.min.y);
      
    }



    public void LateUpdate()
    {
        CameraType();
    }



    /// <summary>
    /// 여러가지 타입을 테스트 해보기 위한 switch 문
    /// </summary>
    private void CameraType()
    {
        switch (CameraMovetype)
        {
            case 0:
                CameraMove_v1();

                break;
            case 1:
                CameraMove_v2();
                break;
        }
    }

    /// <summary>
    /// 카메라가 백그라운드를 넘어가는것을 막는것을 확인하는 함수
    /// </summary>
    /// <param name="size">플레이어의 x,y 값 </param>
    /// <returns>만약 받은 인자 x,y 값이 백그라운드 값을 초과하거나 감소했을경우 최대값, 최소값을 반환해준다.</returns>
    private Vector2 MaxMinSize(Vector2 size)
    {
        
        return new Vector2(
              Mathf.Clamp(size.x, _minsize.x + (_cameraSize.x +2), _maxsize.x - (_cameraSize.x+2))
            , Mathf.Clamp(size.y, _minsize.y + _cameraSize.y, _maxsize.y - _cameraSize.y)
            );
    }   

    /// <summary>
    /// 1번째 이동 테스트 문
    /// </summary>
    private void CameraMove_v1()
    {
        Vector2 playerposition = MaxMinSize(new Vector2(Player.transform.position.x, Player.transform.position.y));

        
        this.transform.position = new Vector3( playerposition.x, playerposition.y + 1,this.transform.position.z );
    }

    /// <summary>
    /// 2번째 이동 테스트 문
    /// </summary>
    private void CameraMove_v2()
    {
        Vector2 playerposition = MaxMinSize(new Vector2(Player.transform.position.x, Player.transform.position.y));

        
        this.transform.position = Vector3.Lerp(transform.position, new Vector3(playerposition.x, playerposition.y + 1, this.transform.position.z),0.03f);
    }
}
