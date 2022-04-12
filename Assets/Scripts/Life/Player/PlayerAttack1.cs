using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack1 : MonoBehaviour
{
    private bool _canAttack = false;

    public GameObject Player;
  

    private List<GameObject> hitEnemyObj = new List<GameObject>();

    public void AnimEventstartAttack()
    {
        Debug.Log("StartEvent");
        hitEnemyObj.Clear();
        _canAttack = true;
    }

    public void AnimEventendAttack()
    {
        
        Debug.Log("EndEvent");
        _canAttack = false;
        HitEnemy();
    }

    public void SkillOneAni()
    {
        Player.GetComponent<Player1>().SkillOne();
    }

    public void SkillTwoAni()
    {
        
        Player.GetComponent<Player1>().SkillTwo();
    }

    public void PlayerStateIdle()
    {
        Player.GetComponent<Player1>().Playerstate = Player1.PlayerstateEnum.Idle;
    }

    public void HitEnemy()
    {
        Debug.Log(hitEnemyObj.Count);
        if(hitEnemyObj.Count > 0) { 
        for(int i = 0; i < hitEnemyObj.Count; i++)
        {
           if(hitEnemyObj[i].GetComponent<I_hp>().Gethit(Player.GetComponent<Life>().Power))
           {
                   //적이 사망
           }
        }
        hitEnemyObj.Clear();
        }
    }

    

    public void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag.Contains("Enemy"))
        {
            if (_canAttack)
            {
                GameObject hitObj = other.gameObject;
                if (!hitEnemyObj.Contains(hitObj)) { 
                    hitEnemyObj.Add(other.gameObject);
                    
                }
            }
        }
    }
}
