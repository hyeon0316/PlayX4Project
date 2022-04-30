using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemy : MonoBehaviour
{
 
   
    public GameObject scriptFill_D;
    public GameObject scriptFill_S;
    public GameObject scriptFill_A;

    void Start()
    {
        
    }
  
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Player_HpGauge.currentFill -= 0.1f;
            scriptFill_A.GetComponent<FillAmount_A>().UseSkill_A();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Enemy_HpGauge.currentFill -= 0.05f;
            scriptFill_S.GetComponent<FillAmount_S>().UseSkill_S();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            scriptFill_D.GetComponent<FillAmount_D>().UseSkill_D();
            //Instantiate(blakc).transform.SetParent(GameObject.Find("Canvas").transform, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("A");
            Player_HpGauge.currentFill += 0.2f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
           
        }
    }
    
}
