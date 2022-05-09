﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
1.public으로 선언 된 변수는 앞글자 대문자로 시작
2. private는 변수 앞에 "_"붙이고  소문자로 시작
3. 함수 안의 지역변수는 네이밍 규칙 상관없음
4. bool형 변수는 is~ or can~ 로 시작
번외로  나중에 구현해야 할 일을 스크립트에 적어 놓을때 //todo: 할 일(메모장 느낌)
*/
public class Life :MonoBehaviour
{
    private int _lifeId;

    public int LifeId
    {
        get { return _lifeId; }
    }

    private float _maxhp;

    public float _Maxhp
    {
        get { return _maxhp; }
    }

    private float _hp;

    public float HpRatio
    {
        get { return _hp / _maxhp; }
    }

    public float HP
    {
        get { return _hp; }
        set { 
            if(value > _maxhp)
            {
                _hp = _maxhp;
            }
            else {
                _hp = value;
            }
            
        }

    }

    private int _power;

    public int Power
    {
        get { return _power; }
        set { _power = value; }
    }
    private float _speed = 5;

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    private bool _living;

    public bool Living
    {
        get { return _living; }
        set { _living = value; }
    }

    /// <summary>
    /// 데이터값을 한번에 입력하는 함수
    /// </summary>
    /// <param name="hp"> 채력값 설정</param>
    /// <param name="power">공격력 설정</param>
    /// <param name="speed">이동속도 설정</param>
    public void Initdata(int id, float hp,int power, float speed)
    {
        _lifeId = id;
        _hp = hp;
        _maxhp = _hp;
        _power = power;
        _speed = speed;
        _living = true;
    }

   

}

public interface I_hp
{

    //todo : 계수를 추가하는 매개변수를 추가하여 데이지를 조절할 수 있도록 설정할것
    /// <summary>
    /// 체력이 감소하는, 증가하는 함수.
    /// </summary>
    /// <param name="Cvalue">변화할 체력양</param>
    ///  <param name="coefficient">들어가는 데미지의 계수</param>
    /// <returns>만약 체력이 0이 된다면 true 를 아니면 false 를 반환한다.</returns>
    bool Gethit(float Cvalue , float coefficient);

    /// <summary>
    /// hp 가 0 이하로 떨어져 죽었는지 확인하는 함수
    /// </summary>
    /// <returns></returns>
    bool CheckLiving();
}

public enum Enemystate
{
    Idle,
    Find,
    Attack,
    Skill,
    Skill2,
    Range,
    Stop,
    Dead
};

public interface I_EnemyControl
{
    Enemystate _enemystate { get; set; }


    IEnumerator DeadAniPlayer();


    void EnemyAttack(float coefficient);


    void EnemyMove();
}


