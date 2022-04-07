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

    [SerializeField] private GameObject _letterBoxParent;
    private RectTransform[] _letterBox;//npc와 대화를 할때 나타나는 상,하 검은색 바

    private float _textDelay = 0.1f;

    [SerializeField] private GameObject _talkPanel;//텍스트의 배경으로 쓰일 패널
    private Text _dialogueText;

    private NpcTalk _npc;

    public bool IsNextTalk = false;
    

    public Image TalkBtnImage;//상호작용 가능을 표시 해 주는 오브젝트
    
    private void Awake()
    {
        _npc = GameObject.Find("NPC").GetComponent<NpcTalk>();
        
        //부모 오브젝트까지 같이 반환,
        //RectTransform으로 가져오는 것이기 때문에
        _letterBox = _letterBoxParent.GetComponentsInChildren<RectTransform>();
    }

    public void TalkStart()
    {
        _dialogueText = _talkPanel.GetComponentInChildren<Text>();
        StartCoroutine(LetterBoxOnCo());
    }


    /// <summary>
    /// 대화내용들을 저장시켜 하나씩 꺼내 사용
    /// </summary>
    /// <param name="lines">대화 내용들</param>
    public void OnDialogue(string[] lines)
    {
        Sentences.Clear();
        foreach (string line in lines)
        {
            Sentences.Enqueue(line);
        }
        NextSentence();
    }

    public void NextSentence()
    {
        //todo: "Skip"이 없을 때의 조건 처리 하기
        if (!Sentences.Peek().Equals("Skip") && Sentences.Count !=0)
        {
            _talkPanel.SetActive(false);
            Invoke("DelayTalk",0.5f);
        }
        else if (Sentences.Count ==0)
        {
            Invoke("ReTalk", 0.5f);
            _talkPanel.SetActive(false);
            StartCoroutine(LetterBoxOffCo());
        }
        else if(Sentences.Peek().Equals("Skip"))
        {
            Debug.Log("Skip을 만났을 때");
            IsNextTalk = true;
            Invoke("ReTalk", 0.5f);
            _talkPanel.SetActive(false);
            StartCoroutine(LetterBoxOffCo());
        }
       
    }

    /// <summary>
    /// 다음 대화 문장이 진행 될때마다 패널이 껏다 켜지기 위함
    /// </summary>
    private void DelayTalk()
    {
        _talkPanel.SetActive(true);
        try
        {
            _currentSentence = Sentences.Dequeue();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        _isTyping = true;
        StartCoroutine(TypingCo(_currentSentence));
    }

    /// <summary>
    /// 대화를 마치고 제자리에서 다시 대화를 할 경우 다시 처음 대사를 출력하기 위해 딜레이용으로 사용
    /// </summary>
    private void ReTalk()
    {
        TalkBtnImage.gameObject.SetActive(true);
        _npc.CanTalk = true;
    }

    /// <summary>
    /// 텍스트를 한글자씩 출력
    /// </summary>
    /// <param name="line">한 글자씩 출력할 문장</param>
    /// <returns></returns>
    IEnumerator TypingCo(string line)
    {
        _dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            _dialogueText.text += letter;
            yield return new WaitForSeconds(_textDelay);
        }
    }
    
    IEnumerator LetterBoxOnCo()
    {
        float time = 0f;
        while (time <= 1.0f)
        {
            time += Time.deltaTime* 2;
            LetterBoxMove(time);
            yield return null;
        }
    }
    
    IEnumerator LetterBoxOffCo()
    {
        float time = 1f;
        while (time >= 0f)
        {
            time -= Time.deltaTime * 2;
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
        Debug.Log(Sentences.Count);
        TalkCheck();
    }

    private void TalkCheck()
    {
        if (TalkBtnImage.gameObject.activeSelf)
        {
            TalkBtnImage.transform.position = _npc.transform.position + new Vector3(0f, 1f, 0.5f);
        }
        
        if (_talkPanel.activeSelf)
        {
            _talkPanel.transform.position = _npc.transform.position + new Vector3(0.8f, 1.2f, 0.5f);
            
            //텍스트가 전부 채워졌을때
            if (_dialogueText.text.Equals(_currentSentence))
            {
                _isTyping = false;
            }

            //대화 진행
            if (!_isTyping && Input.GetKeyDown(KeyCode.Space))
            {
                NextSentence();
            }
        }
    }
}
