using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;



public class SystemBase : MonoBehaviour
{
    public GameObject SystemWindow;
    public GameObject ManualWindow;
    public GameObject ManualPages;
    private bool _isActivate;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !ManualWindow.activeSelf)
        {
            _isActivate = !_isActivate;
            OpenSystem(_isActivate);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && ManualWindow.activeSelf)
        {
            OpenControls(false);
        }
    }
    
    private void OpenSystem(bool activate)
    {
        SystemWindow.SetActive(activate);
    }

    private void OpenControls(bool activate)
    {
        ManualWindow.SetActive(activate);
    }

    public void ResumeBtn()
    {
        _isActivate = false;
        OpenSystem(_isActivate);
    }

    public void ControlsBtn()
    {
        OpenControls(true);
    }

    public void QuitBtn()
    {
        SceneManager.LoadScene("Menu");
    }

    public void SoundBtn()
    {
        //todo: 사운드 조절 버튼
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
        OpenControls(false);
    }
    

}
