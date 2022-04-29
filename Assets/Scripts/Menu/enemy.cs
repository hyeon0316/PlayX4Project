using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemy : MonoBehaviour
{
 
    public GameObject blakc;
    public GameObject skill_black;
    public GameObject scriptFill;


    void Start()
    {
        
    }
  
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Player_HpGauge.currentFill -= 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Enemy_HpGauge.currentFill -= 0.05f;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {

            scriptFill.GetComponent<FillAmount>().UseSkill();
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
