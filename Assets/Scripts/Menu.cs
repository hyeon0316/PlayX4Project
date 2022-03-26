using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{


 public void OnClick()
    {
        SceneManager.LoadScene("TestScenes");
    }
 
    public void OnClickQuitGame()
    {
       #if UNNITY_EDITOR
       UnityEditor.EditorAppliiiication.isPlaying =false;
# else
        Application.Quit();
#endif
    }
}
