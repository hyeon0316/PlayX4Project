using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Power;

    public float Speed;

    private float _offTime;
    public void Update()
    {
        _offTime += Time.deltaTime;
        if(_offTime >= 5f)
        {
            _offTime = 0;
            EndBullet();
        }

        this.transform.Translate(Vector3.right * Time.deltaTime * Speed);
    }

    public void StartBullet()
    {
        this.gameObject.SetActive(true);
    }

    public void EndBullet()
    {
        this.gameObject.SetActive(false);
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("bullet 히트");
            other.GetComponent<I_hp>().Gethit(Power);
            this.GetComponent<Animator>().SetTrigger("impact");
        }
    }
}
