using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadeImage : MonoBehaviour, IFade
{
    private Image _fadeImage;
    public float FadeCount;
    
    private void OnEnable()
    {
        _fadeImage = GetComponent<Image>();
    }


    public void FadeIn()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeInCo());
    }

    public void FadeOut()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeOutCo());
    }
    
    IEnumerator FadeInCo()
    {
        FadeCount = 0f;
        while (FadeCount <= 1.0f)
        {
            FadeCount += 0.01f;
            yield return new WaitForSeconds(0.01f);
            _fadeImage.color = new Color(0, 0, 0, FadeCount);
        }
    }

    IEnumerator FadeOutCo()
    {
        FadeCount = 1f;
        while (FadeCount >= 0)
        {
            FadeCount -= 0.01f;
            yield return new WaitForSeconds(0.01f);
            _fadeImage.color = new Color(0, 0, 0, FadeCount);
        }
        gameObject.SetActive(false);
    }
}