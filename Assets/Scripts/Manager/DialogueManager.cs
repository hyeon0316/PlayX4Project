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

    //[SerializeField]
    private Text _dialogueText;

    [SerializeField] private GameObject _letterBoxParent;
    private RectTransform[] _letterBox;//npc와 대화를 할때 나타나는 상,하 이미지

    private float _textDelay = 1f;

    private void Awake()
    {
        _letterBox = _letterBoxParent.GetComponentsInChildren<RectTransform>();//부모 오브젝트까지 같이 반환,
                                                                               //RectTransform으로 가져오는 것이기 때문에
    }

    private void Start()
    {
        StartCoroutine(TalkOnCo());
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
            StartCoroutine(TypingCo(_currentSentence));
        }
    }

    IEnumerator TypingCo(string line)
    {
        _dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            _dialogueText.text += letter;
            yield return new WaitForSeconds(_textDelay);
        }
    }
    
    IEnumerator TalkOnCo()
    {
        float time = 0f;
        while (time <= 1.0f)
        {
            time += Time.deltaTime;
            LetterBoxMove(time);
            yield return null;
        }
    }
    
    IEnumerator TalkOffCo()
    {
        float time = 1f;
        while (time >= 0f)
        {
            time -= Time.deltaTime;
            LetterBoxMove(time);
            yield return null;
        }
    }
    

    /// <summary>
    /// 레터박스를 움직이는 함수
    /// </summary>
    /// <param name="value">값이 0일 때 레터박스가 사라지고 1일 때 보임</param>
    private void LetterBoxMove(float value)
    {
        _letterBox[1].anchoredPosition = Vector2.Lerp(new Vector2(_letterBox[1].anchoredPosition.x, -150),
            new Vector2(_letterBox[1].anchoredPosition.x, 0), value);
        
        _letterBox[2].anchoredPosition = Vector2.Lerp(new Vector2(_letterBox[2].anchoredPosition.x, 150),
            new Vector2(_letterBox[2].anchoredPosition.x, 0), value);
    }
    
    private void Update()
    {
        //텍스트가 전부 채워졌을때
        //if (_dialogueText.text.Equals(_currentSentence))
        //{
            
        //}
    }
    //todo: 나중에는 오브젝트마다 NpcTalk 스크립트를 붙여놓고 대사 자동 진행 or 수동 진행 
}
