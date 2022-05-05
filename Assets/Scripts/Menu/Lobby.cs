using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    public Queue<string> Sentences = new Queue<string>();
    
    public GameObject ManualWindow;
    public GameObject ManualPages;
    public GameObject EventWindow;
    private Text _eventText;
    public string[] IntroSentences;
    private string _currentSentence;

    private float _textDelay = 0.1f;
    private void Awake()
    {
        _eventText = EventWindow.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        Debug.Log(Sentences.Count);
    }

    public void StartGame()
    {
        //todo: 검은이미지 배경에 텍스트 출력 이후 씬 이동
        EventWindow.SetActive(true);
        OnDialogue(IntroSentences);
    }
    
    public void OnDialogue(string[] lines)
    {
        Sentences.Clear();
        foreach (string line in lines)
        {
            Sentences.Enqueue(line);
        }
        NextSentence();
    }

    private void NextSentence()
    {
        if (Sentences.Count != 0 && !Sentences.Peek().Equals("Next") && !Sentences.Peek().Equals("Start"))
        {
            PrintSentences();
        }
        else if (Sentences.Peek().Equals("Next"))
        {
            Debug.Log("줄 바꿈");
            Sentences.Dequeue();
            _eventText.text += "\n" + "\n";
            PrintSentences();
        }
        else if (Sentences.Peek().Equals("Start"))
        {
            SceneManager.LoadScene("Town");
        }
    }
    private void PrintSentences()
    {
        _currentSentence = Sentences.Dequeue();
        StartCoroutine(IntroCo(_currentSentence));
    }

    private IEnumerator IntroCo(string line)
    {
        yield return new WaitForSeconds(1f);
        foreach (char letter in line.ToCharArray())
        {
            _eventText.text += letter;
            yield return new WaitForSeconds(_textDelay);
        }
        NextSentence();
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
