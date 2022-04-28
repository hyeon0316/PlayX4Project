using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class EnemyCounter : MonoBehaviour
{
    public int EnemyPosIndex;
    public int WalfIndex;

    private bool _isFreeze;

    private GameManager _gameManager;

    public static bool IsPlayerStop;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        if (this.transform.childCount != 0)
        {
            if (this.transform.name.Equals("EnemyPos_Second"))
            {
                Debug.Log("2층");
                _gameManager.PlayCutScene();
                IsPlayerStop = true;
            }
            _isFreeze = true;
        }
    }

    private void Update()
    {
        if (this.transform.childCount == 0 && _isFreeze)
        {
            _gameManager.ActivateWalf(EnemyPosIndex,WalfIndex);
            _isFreeze = false;
        }

        if (this.transform.name.Equals("EnemyPos_Second") && IsPlayerStop)
        {
            FindObjectOfType<Player>().IsStop = true;
        }
    }
}
