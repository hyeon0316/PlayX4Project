using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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


    public void StartGame()
    {
        FindObjectOfType<SoundManager>().Play("Object/Button",SoundType.Effect);
        FindObjectOfType<SoundManager>().Play("TownBGM", SoundType.Bgm);
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

    private void DelayLoad()
    {
        SceneManager.LoadScene("Town");
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
            Invoke("DelayLoad", 2f);
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
            int rand = Random.Range(0, 5);
            switch (rand)
            {
                case 0:
                    FindObjectOfType<SoundManager>().Play("Object/IntroTyping1",SoundType.Effect);
                    break;
                case 1:
                    FindObjectOfType<SoundManager>().Play("Object/IntroTyping2",SoundType.Effect);
                    break;
                case 2:
                    FindObjectOfType<SoundManager>().Play("Object/IntroTyping3",SoundType.Effect);
                    break;
                case 3:
                    FindObjectOfType<SoundManager>().Play("Object/IntroTyping4",SoundType.Effect);
                    break;
                case 4:
                    FindObjectOfType<SoundManager>().Play("Object/IntroTyping5",SoundType.Effect);
                    break;
            }
            yield return new WaitForSeconds(_textDelay);
        }
        NextSentence();
    }

    public void ShowManual()
    {
        FindObjectOfType<SoundManager>().Play("Object/Button",SoundType.Effect);
        ManualWindow.SetActive(true);
    }
    public void ExitGame()
    {
        FindObjectOfType<SoundManager>().Play("Object/Button",SoundType.Effect);
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void NextPageBtn()
    {
        FindObjectOfType<SoundManager>().Play("Object/Button",SoundType.Effect);
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
        FindObjectOfType<SoundManager>().Play("Object/Button",SoundType.Effect);
        ManualWindow.SetActive(false);
    }
}
