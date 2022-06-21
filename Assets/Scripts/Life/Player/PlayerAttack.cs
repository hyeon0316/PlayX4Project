﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackHitSoundType
{
    XHit,
    ZHit,
    AHit,
    SHit,
    DHit,
    Empty
}
public class PlayerAttack : MonoBehaviour
{
    public AttackHitSoundType Type;
    private bool _canAttack = false;

    public GameObject Player;


    private List<GameObject> hitEnemyObj = new List<GameObject>();

    public void AnimEventstartAttack()
    {
        Debug.Log("StartEvent");
        hitEnemyObj.Clear();
        _canAttack = true;
    }

    public void AnimEventendAttack(float coefficient)
    {

        Debug.Log("EndEvent");
        _canAttack = false;
        HitEnemy(coefficient);
        hitEnemyObj.Clear();
    }

    public void AnimEventKnockback(AnimationEvent animationEvent)//애니메이션 이벤트가 한개만 들어간다.
    {
        
        _canAttack = false;
        knockbackEnemy(animationEvent.intParameter,animationEvent.floatParameter);
    }

    public void SkillOneAni()
    {
        Player.GetComponent<Player>().SkillOne();
    }

    public void SkillTwoAni()
    {

        Player.GetComponent<Player>().SkillTwo();
    }

    public void PlayerThreeAni()
    {
        Debug.Log("ThreeAni");
        Player.GetComponent<Player>().SkillThree(hitEnemyObj);
    }

    public void PlayerStateIdle()
    {
        Player.GetComponent<Player>().Playerstate = global::Player.PlayerstateEnum.Idle;
    }
    public void PlayerStateAttack()
    {
        Player.GetComponent<Player>().Playerstate = global::Player.PlayerstateEnum.Attack;
    }


    /// <summary>
    /// 적 히트 성공시 따로 필요한 사운드 재생
    /// </summary>
    /// <param name="type"></param>
    public void SelectHitSound(AttackHitSoundType type)
    {
        for (int i = 0; i < hitEnemyObj.Count; i++)
        {
            if (hitEnemyObj[i] == null) continue;
            hitEnemyObj[i].GetComponent<I_hp>().SelectHit(type);
        }
    }
    public void HitEnemy(float coefficient)
    {
        bool hitBoss = false;
        Debug.Log(hitEnemyObj.Count);
        if (hitEnemyObj.Count > 0)
        {
            for(int i = 0; i < hitEnemyObj.Count; i++)
            {
                if(hitEnemyObj[i].GetComponent<Life>().LifeId >= 10)
                {
                    hitBoss = true;
                }
            }

            for (int i = 0; i < hitEnemyObj.Count; i++)
            {
                if (hitEnemyObj[i] == null) continue;
                float beforehp = hitEnemyObj[i].GetComponent<Life>().HpRatio;
                if (hitEnemyObj[i].GetComponent<I_hp>().Gethit(Player.GetComponent<Life>().Power, coefficient))
                {
                    if (hitBoss)
                    {
                        if (hitEnemyObj[i].GetComponent<Life>().LifeId >= 10)
                            GameObject.Find("Canvas(Enemy)").GetComponent<EnemyHpbar>().SwitchHPbar(hitEnemyObj[i].GetComponent<Life>().LifeId, hitEnemyObj[i].GetComponent<Life>().HpRatio, beforehp, true);
                    }
                    else
                    {
                        GameObject.Find("Canvas(Enemy)").GetComponent<EnemyHpbar>().SwitchHPbar(hitEnemyObj[i].GetComponent<Life>().LifeId, hitEnemyObj[i].GetComponent<Life>().HpRatio, beforehp, true);
                    }
                    
                }
                else
                {
                    if (hitBoss) { 
                        if(hitEnemyObj[i].GetComponent<Life>().LifeId >= 10)
                            GameObject.Find("Canvas(Enemy)").GetComponent<EnemyHpbar>().SwitchHPbar(hitEnemyObj[i].GetComponent<Life>().LifeId, hitEnemyObj[i].GetComponent<Life>().HpRatio, beforehp);
                    }
                    else
                    {
                        GameObject.Find("Canvas(Enemy)").GetComponent<EnemyHpbar>().SwitchHPbar(hitEnemyObj[i].GetComponent<Life>().LifeId, hitEnemyObj[i].GetComponent<Life>().HpRatio, beforehp);
                    }
                }
            }
           
        }
    }

    public void knockbackEnemy(int id,float Power)
    {
        if(hitEnemyObj.Count > 0) { 
            for(int i = 0; i < hitEnemyObj.Count; ++i)
            {
                if (hitEnemyObj[i].GetComponent<Life>().LifeId >= 10) continue;

                

                Debug.Log("넉백");
                StartCoroutine(hitEnemyObj[i].GetComponent<Life>().Navstop(0.065f * Power));
                StartCoroutine(hitEnemyObj[i].GetComponent<Life>().AnimStop(0.03f * Power));
                StartCoroutine(hitEnemyObj[i].GetComponent<Life>().GravityStop(0.03f));
                if(id == 0) { 
                    hitEnemyObj[i].GetComponent<Life>().KnockBackRight(Player.transform.position, Power);
                }
                else if(id == 1)
                {
                    hitEnemyObj[i].GetComponent<Life>().KnockBackUp(Player.transform.position, Power);
                }else if(id == 2)
                {
                    hitEnemyObj[i].GetComponent<Life>().KnockBackRightUp(Player.transform.position, Power);
                }
            }
        }
    }

    public void EnemyAnistop(float time)
    {
        for(int i = 0; i < hitEnemyObj.Count; i++)
        {
            StartCoroutine(hitEnemyObj[i].GetComponent<Life>().AnimStop(time));
            StartCoroutine(hitEnemyObj[i].GetComponent<Life>().GravityStop(time));
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Contains("Enemy"))
        {
            if (_canAttack)
            {
                if(other.GetComponent<Life>().HpRatio > 0) { 
                    GameObject hitObj = other.gameObject;
                    if (!hitEnemyObj.Contains(hitObj))
                    {
                        hitEnemyObj.Add(other.gameObject);

                    }
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Contains("Enemy"))
        {
            if (_canAttack)
            {
                GameObject hitObj = other.gameObject;
                if (hitEnemyObj.Contains(hitObj))
                {
                    hitEnemyObj.Remove(hitObj);
                }
            }
        }
    }

}
