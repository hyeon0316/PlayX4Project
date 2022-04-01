using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadeImage : MonoBehaviour, IFade
{
    private Image _fadeImage;

    private void Awake()
    {
        _fadeImage = GetComponent<Image>();
    }

    public void FadeIn()
    {
        _fadeImage.enabled = true;
        StartCoroutine(FadeInCo());
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCo());
        _fadeImage.enabled = false;
    }
    
    IEnumerator FadeInCo()
    {
        float fadeCount = 0f;
        while (fadeCount <= 1.0f)
        {
            fadeCount += 0.01f;
            yield return new WaitForSeconds(0.01f);
            _fadeImage.color = new Color(0, 0, 0, fadeCount);
        }
    }

    IEnumerator FadeOutCo()
    {
        float fadeCount = 1f;
        while (fadeCount > 0)
        {
            fadeCount -= 0.01f;
            yield return new WaitForSeconds(0.01f);
            _fadeImage.color = new Color(0, 0, 0, fadeCount);
        }
    }
}