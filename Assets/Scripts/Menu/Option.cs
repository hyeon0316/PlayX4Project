using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Option : MonoBehaviour
{
    public GameObject Option_page;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Option_page.SetActive(true);
            Time.timeScale = 0;
        }
    }
    public void OptionDown()
    {
        // Time.timeScale = 0;  //시간 정지
        Option_page.SetActive(true);
    }
    public void Option_Exit()
    {
        Option_page.SetActive(false);
        Time.timeScale = 1; 

    }
}
