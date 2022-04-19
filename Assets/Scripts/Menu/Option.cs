using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Option : MonoBehaviour
{

    bool  IsPause;

    public GameObject Quit;
    public GameObject Option_page;
    public GameObject Menu_Page;
    public string thisScene;
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
            thisScene = SceneManager.GetActiveScene().name;
            if (IsPause == false)
            {
                Menu_Page.SetActive(true);
                Time.timeScale = 0;  //시간 정지
                IsPause = true;
                return;
            }

            if(IsPause ==true)
            {     Menu_Page.SetActive(false);
                Option_page.SetActive(false);
                Time.timeScale = 1;
                IsPause = false;
                return;
            }
        }
    }

    public void Restart()
        {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(thisScene);
    }
    public void OptionDown()
    {
        Option_page.SetActive(true);
        Menu_Page.SetActive(false);
    }
    public void Menu_Option()
    {
        Menu_Page.SetActive(true);
    }
    public void Option_Quit()
    { 
        Quit.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Quit(B)", typeof(Sprite)) as Sprite;
        Option_page.SetActive(false);
        Menu_Page.SetActive(false);


        if (IsPause == true)
        {
            Option_page.SetActive(false);
            Menu_Page.SetActive(false);
            Time.timeScale = 1;
            IsPause = false;
            return;
        }
    }

  public void Eixt()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
  }