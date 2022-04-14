using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Option : MonoBehaviour
{

    bool  IsPause;

    public GameObject Exit;
    public GameObject Option_page;
 void Start()
    {
        {
            IsPause = false;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPause == false)
            {
                Option_page.SetActive(true);
                Time.timeScale = 0;  //시간 정지
                IsPause = true;
                return;
            }

            if(IsPause ==true)
            {     Option_page.SetActive(false);
                Time.timeScale = 1;
                IsPause = false;
                return;
            }
        }
    }
    public void OptionDown()
    {
        Option_page.SetActive(true);
    }
    public void Option_Exit()
    { 
        Exit.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Exit(B)", typeof(Sprite)) as Sprite;
        Option_page.SetActive(false);


        if (IsPause == true)
        {
            Option_page.SetActive(false);
            Time.timeScale = 1;
            IsPause = false;
            return;
        }


    }
    
}
