using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public GameObject[] Walf;
    
    public GameObject[] EnemyPos;

    public PlayableDirector playableDirector;
    public TimelineAsset[] Timeline;
    
    // Start is called before the first frame update
    void Start()
    {
        //InitTimeline();
    }

   

    private void InitTimeline()
    {
        playableDirector.Pause();
    }

   
    /// <summary>
    /// 다른 콜리더는 끄고 필요한 콜리더만 켜주는 함수
    /// </summary>
    /// <param name="nextCollider">활성화 시킬 콜리더</param>
    public void ActivateCollider(string nextCollider)
    {
        for (int i = 0; i < GameObject.Find("Colliders").transform.childCount; i++)
        {
            GameObject.Find("Colliders").transform.GetChild(i).gameObject.SetActive(false);
        }
        GameObject.Find("Colliders").transform.Find(nextCollider).gameObject.SetActive(true);
    }

    /// <summary>
    /// 각 맵의 몬스터들을 클리어시 워프를 사용 가능
    /// </summary>
    public void ActivateWalf(int enemyPosIndex,int walfIndex)
    {
        if (EnemyPos[enemyPosIndex].transform.childCount == 0)
        {
            Walf[walfIndex].SetActive(true);
            if(enemyPosIndex == 0)
                Walf[++walfIndex].SetActive(true);
        }
    }

    public void PlayCutScene(int timelieIndex)
    {
        playableDirector.Play(Timeline[timelieIndex]);
    }
}
