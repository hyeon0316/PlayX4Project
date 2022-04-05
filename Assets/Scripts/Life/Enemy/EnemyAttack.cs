using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private bool _isHitPlayer;

    public bool IshitPlayer
    {
        get { return _isHitPlayer; }
    }

   public void Attackhit()
    {
        this.transform.parent.GetComponent<I_EnemyControl>().EnemyAttack();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Contains("Player"))
        {
            _isHitPlayer = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.transform.tag.Contains("Player"))
        {
            _isHitPlayer = false;
        }
    }



}
