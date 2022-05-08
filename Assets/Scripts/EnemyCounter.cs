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
                _gameManager.PlayCutScene(1);
                IsPlayerStop = true;
            }
            else if (this.transform.name.Equals("EnemyPos_Boss"))
            {
                //todo:보스 컷씬
                FindObjectOfType<SoundManager>().Play("BossBGM",SoundType.Bgm);
                _gameManager.PlayCutScene(2);
                IsPlayerStop = true;
            }
            _isFreeze = true;
        }
    }

    private void Update()
    {
        if (this.transform.childCount == 0 && _isFreeze)
        {
            if (this.transform.name.Equals("EnemyPos_Second"))
            {
                FindObjectOfType<Inventory>().AddMaterial("PrisonKey");
            }
            _gameManager.ActivateWalf(EnemyPosIndex,WalfIndex);
            _isFreeze = false;
        }

        if (IsPlayerStop)
        {
            FindObjectOfType<Player>().IsStop = true;
        }
    }
}
