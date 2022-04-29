using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
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

    public void AnimEventendAttack(float coefficient)
    {

        Debug.Log("EndEvent");
        _canAttack = false;
        HitEnemy(coefficient);
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


    public void HitEnemy(float coefficient)
    {
        Debug.Log(hitEnemyObj.Count);
        if (hitEnemyObj.Count > 0)
        {
            for (int i = 0; i < hitEnemyObj.Count; i++)
            {
                if (hitEnemyObj[i] == null) continue;

                if (hitEnemyObj[i].GetComponent<I_hp>().Gethit(Player.GetComponent<Life>().Power, coefficient))
                {
                    //적이 사망
                }
            }
            hitEnemyObj.Clear();
        }
    }



    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Contains("Enemy"))
        {
            if (_canAttack)
            {
                GameObject hitObj = other.gameObject;
                if (!hitEnemyObj.Contains(hitObj))
                {
                    hitEnemyObj.Add(other.gameObject);

                }
            }
        }
    }
}
