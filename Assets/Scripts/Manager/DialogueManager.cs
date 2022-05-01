using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Queue<string> Sentences = new Queue<string>();
    private bool _isTyping = false;
    private string _currentSentence;

    [SerializeField] private GameObject _letterBoxParent;
    private RectTransform[] _letterBox;//npc와 대화를 할때 나타나는 상,하 검은색 바
    private float _textDelay = 0.1f;
    private Text _dialogueText;
    public NpcTalk Npc;

    private Player _player;

    public bool IsNextTalk = false;

    public GameObject TalkPanel;

    private GameManager _gameManager;
        
    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        TalkPanel = GameObject.Find("UICanvas").transform.Find("TalkPanel").gameObject;
        _player = GameObject.Find("Player").GetComponent<Player>();
        //부모 오브젝트까지 같이 반환,
        //RectTransform으로 가져오는 것이기 때문에
        _letterBox = _letterBoxParent.GetComponentsInChildren<RectTransform>();
    }
    private void Start()
    {
        GameObject.Find("Canvas").transform.Find("FadeImage").GetComponent<FadeImage>().FadeOut();
    }
    private void Update()
    {
        TalkCheck();
    }

    private void TalkCheck()
    {
        if (TalkPanel.activeSelf)
        {
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
    
    public void TalkStart()
    {
        _dialogueText = TalkPanel.GetComponentInChildren<Text>();
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
        if (!Sentences.Peek().Equals("Delete") && !Sentences.Peek().Equals("Stop"))
        {
            TalkPanel.SetActive(false);
            Invoke("DelayTalk",0.5f);
        }
        else if(Sentences.Peek().Equals("Delete"))
        {
            List<string> tmp = new List<string>(Npc.Sentences);
            for (int i = 0; i < tmp.Count; i++)
            {
                i = 0;
                if (tmp[i].Equals("Delete"))
                {
                    tmp.RemoveAt(i);
                    break;
                }
                tmp.RemoveAt(i);
            }
            Npc.Sentences = tmp.ToArray();
            CloseTalkPanel();
            if (Npc.transform.Find("Captain") || Npc.transform.Find("Lady"))
            {
                _gameManager.Walf[0].SetActive(true);
            }
            
        }
        else if (Sentences.Peek().Equals("Stop"))
        {
            CloseTalkPanel();
            Necromancer.IsCutScene = false;
        }
    }

    /// <summary>
    /// 대화창이 꺼질때 발생하는 이벤트들
    /// </summary>
    private void CloseTalkPanel()
    {
        FindObjectOfType<CameraManager>().Target = _player.gameObject;
        Invoke("ReTalk", 0.5f);
        TalkPanel.SetActive(false);
        StartCoroutine(LetterBoxOffCo());
    }
    
    
    /// <summary>
    /// 다음 대화 문장이 진행 될때마다 패널이 껏다 켜지기 위함
    /// </summary>
    private void DelayTalk()
    {
        TalkPanel.SetActive(true);
        _currentSentence = Sentences.Dequeue();
        _isTyping = true;
        StartCoroutine(TypingCo(_currentSentence));
    }

    /// <summary>
    /// 대화를 마치고 제자리에서 다시 대화를 할 경우 다시 처음 대사를 출력하기 위해 딜레이용으로 사용
    /// </summary>
    private void ReTalk()
    {
        //todo: 보스방에서 함수 실행시 오류나므로 조건문 걸어줘야함
        Npc.ActionBtn.SetActive(true);
        Npc.CanInteract = true;
    }

    /// <summary>
    /// 텍스트를 한글자씩 출력
    /// </summary>
    /// <param name="line">한 글자씩 출력할 문장</param>
    /// <returns></returns>
    private IEnumerator TypingCo(string line)
    {
        _dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            _dialogueText.text += letter;
            yield return new WaitForSeconds(_textDelay);
        }
    }
    
    private IEnumerator LetterBoxOnCo()
    {
        _player.IsStop = true;
        float time = 0f;
        while (time <= 1.0f)
        {
            time += Time.deltaTime * 2;
            LetterBoxMove(time);
            yield return null;
        }
    }
    
    private IEnumerator LetterBoxOffCo()
    {
        float time = 1f;
        while (time >= 0f)
        {
            time -= Time.deltaTime * 2;
            LetterBoxMove(time);
            yield return null;
        }
        _player.IsStop = false;
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
}
