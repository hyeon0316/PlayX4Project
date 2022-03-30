using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//보스 관련 스크립트
public class Boss : Life, I_hp
{
    private int _round;

    public void Awake()
    {
        Initdata(0, 0, 0);
        _round = 2;
    }

    public void Update()
    {
        //패턴 돌리기 시작
    }


    private void PlayPatten()
    {
        //라운드마다 패턴 분리
        switch (_round)
        {
            case 3:
                break;
            case 2:
                break;
            case 1:
                break;
            case 0:
                break;
        }
    }



    public bool Gethit(int Cvalue)
    {

        HP -= Cvalue;
        //다음 라운드가 있다면 체력을 최대로 회복한 후 _round를 1줄인다. _round 가0일때 마지막 라운드
        if(HP <=0 && _round > 0)
        {
            _round -= 1;
            HP = _Maxhp;
        }



        return CheckLiving();
    }

    public bool CheckLiving()
    {

        if (HP <= 0)
            return true;
        else
            return false;
    }





}
