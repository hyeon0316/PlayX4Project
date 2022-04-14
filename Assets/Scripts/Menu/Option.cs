using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    public GameObject Exit;
    public GameObject Option_page;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Option_page.SetActive(true);
            Time.timeScale = 0;  //시간 정지
        }
    }
    public void OptionDown()
    {
        Option_page.SetActive(true);
    }
    public void Option_Exit()
    {   Exit.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Exit(B)", typeof(Sprite)) as Sprite;
        Option_page.SetActive(false);
        
        Time.timeScale = 1;
       
    }
    
}
