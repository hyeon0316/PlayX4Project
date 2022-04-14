using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    public GameObject Option_page;

    public void OnClick()
    {
        SceneManager.LoadScene("TestScenes");
    }

  
    public void OnClickQuitGame()
    {
      
        Application.Quit();
    }
}
