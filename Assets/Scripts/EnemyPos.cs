using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class EnemyPos : MonoBehaviour
{
    public int EnemyPosIndex;
    public int WalfIndex;

    private bool _isFreeze;

    private void OnEnable()
    {
        if (this.transform.childCount != 0)
        {
            if (this.transform.name.Equals("EnemyPos_Second"))
            {
                //todo: 처음 들어갔을 때 적들이 1명이상이므로 이때 연출씬 실행 그 이후 적 클리어시 0명이기 때문에 발동안됨
                //todo: 연출씬 실행 도중에는 인게임 조작 불가
                Debug.Log("2층");
                //FindObjectOfType<GameManager>().PlayCutScene(0);
            }

            _isFreeze = true;
        }
    }

    private void Update()
    {
        if (this.transform.childCount == 0 && _isFreeze)
        {
            FindObjectOfType<GameManager>().ActivateWalf(EnemyPosIndex,WalfIndex);
            _isFreeze = false;
        }
    }
}
