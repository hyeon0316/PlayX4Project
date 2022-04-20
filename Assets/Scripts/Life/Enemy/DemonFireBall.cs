using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonFireBall : MonoBehaviour
{
    public int Power;

    public float Speed = 2f;
    
    private float _timer;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<I_hp>().Gethit(Power);
            GameObject.Find("Demon_Page2").GetComponent<Demon>().ReturnFireBall(this.gameObject);
        }
    }
    
    private void OnEnable()
    {
        _timer = 0;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > 10f)
        {
            GameObject.Find("Demon_Page2").GetComponent<Demon>().ReturnFireBall(this.gameObject);
        }
        this.transform.Translate(Vector3.left* Speed * Time.deltaTime);
    }
}
