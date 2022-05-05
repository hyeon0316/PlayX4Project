using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;



public class SystemBase : MonoBehaviour
{
    public GameObject SystemWindow;
    public GameObject ManualWindow;
    private bool _isPause;
    private bool _isActivate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

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
        CheckPause();
    }
    
    private void OpenSystem(bool activate)
    {
        SystemWindow.SetActive(activate);
        _isPause = activate;
    }

    private void OpenControls(bool activate)
    {
        ManualWindow.SetActive(activate);
    }

    private void CheckPause()
    {
        if (_isPause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void ResumeBtn()
    {
        _isActivate = false;
        OpenSystem(_isActivate);
    }

    public void ControlsBtn()
    {
        OpenControls(true);
        //todo: 조작법 창 열기
    }

    public void QuitBtn()
    {
        SceneManager.LoadScene("Menu");
    }

    public void SoundBtn()
    {
        //todo: 사운드 조절 버튼
    }
    

}
