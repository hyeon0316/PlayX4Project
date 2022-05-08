﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class SystemBase : MonoBehaviour
{
    public GameObject SystemWindow;
    public GameObject ManualWindow;
    public GameObject ManualPages;
    private bool _isActivate;
    public Slider BgmSlider;
    public Slider EffectSlider;
    private bool _canAdjust;

    public float BgmVolume = 1;
    public float EffectVolume = 1;
    
    private static SystemBase _instance = null;

    private void Awake()
    {
        if (_instance == null && !SceneManager.GetActiveScene().name.Equals("Menu"))
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else 
        {
            if (this != _instance)
            {
                Destroy(this.gameObject);
            }
        }
    }
    
    private void Start()
    {
        BgmSlider.value = BgmVolume;
        EffectSlider.value = EffectVolume;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !ManualWindow.activeSelf)
        {
            _isActivate = !_isActivate;
            OpenSystem(_isActivate);
            CloseSoundBtn();
            BgmSlider.gameObject.SetActive(false);
            EffectSlider.gameObject.SetActive(false);
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
        _canAdjust = !_canAdjust;
        BgmSlider.gameObject.SetActive(_canAdjust);
        EffectSlider.gameObject.SetActive(_canAdjust);
    }

    private void CloseSoundBtn()
    {
        _canAdjust = false;
        BgmSlider.gameObject.SetActive(_canAdjust);
        EffectSlider.gameObject.SetActive(_canAdjust);
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
