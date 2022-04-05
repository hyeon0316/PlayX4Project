using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
    public Queue<string> Sentences = new Queue<string>();
    private bool _isTyping = false;
    private string _currentSentence;

    [SerializeField] private Text _dialogueText;

    [SerializeField] private GameObject _talkModeParent;
    private RectTransform[] _talkMode;//npc와 대화를 할때 나타나는 상,하 이미지

    private float _textDelay = 1f;

    private void Awake()
    {
        _talkMode = _talkModeParent.GetComponentsInChildren<RectTransform>();
    }

    
    public void OnDialogue(string[] lines)
    {
        Sentences.Clear();
        foreach (string line in lines)
        {
            Sentences.Enqueue(line);
        }
    }

    public void NextSentence()
    {
        if (Sentences.Count != 0)
        {
            _currentSentence = Sentences.Dequeue();
            StartCoroutine(Typing(_currentSentence));
        }
    }

    IEnumerator Typing(string line)
    {
        _dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            _dialogueText.text += letter;
            yield return new WaitForSeconds(_textDelay);
        }
    }

    
    private void Update()
    {
        //텍스트가 전부 채워졌을때
        if (_dialogueText.text.Equals(_currentSentence))
        {
            
        }
    }
    //todo: 나중에는 오브젝트마다 NpcTalk 스크립트를 붙여놓고 대사 자동 진행 or 수동 진행 
}
