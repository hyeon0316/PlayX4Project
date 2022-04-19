using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{


    public void OnClick()
    {
        SceneManager.LoadScene("Town");
    }

  
    public void OnClickQuitGame()
    {
      
        Application.Quit();
    }
}
