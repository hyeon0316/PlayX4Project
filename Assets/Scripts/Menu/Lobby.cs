using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Lobby : MonoBehaviour
{
    public GameObject ManualWindow;
    public GameObject ManualPages;
    public void StartGame()
    {
        SceneManager.LoadScene("Town");
    }

    public void ShowManual()
    {
        ManualWindow.SetActive(true);
    }
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void NextPageBtn()
    {
        if (ManualPages.transform.GetChild(0).gameObject.activeSelf)
        {
            ManualPages.transform.GetChild(0).gameObject.SetActive(false);
            ManualPages.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            ManualPages.transform.GetChild(0).gameObject.SetActive(true);
            ManualPages.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    public void BackBtn()
    {
        ManualWindow.SetActive(false);
    }
}
