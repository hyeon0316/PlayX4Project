using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcTalk : MonoBehaviour
{
    public string[] Sentences_First;
    public bool CanTalk = false;

    private DialogueManager _dialogueManager;

    private void Awake()
    {
        _dialogueManager = GameObject.Find("Canvas").GetComponent<DialogueManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.Find("UICanvas").transform.Find("ActionBtn").gameObject.SetActive(true);
            Debug.Log("대화 가능");
            CanTalk = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("대화 불가능");
        GameObject.Find("UICanvas").transform.Find("ActionBtn").gameObject.SetActive(false);
        CanTalk = false;
    }

    private void Update()
    {
        TalkStart();
    }

    private void TalkStart()
    {
        if (CanTalk)
        {
            GameObject.Find("UICanvas").transform.Find("ActionBtn").transform.position = this.transform.position + new Vector3(0f, 1f, 0.5f);
        }
        
        if (CanTalk && Input.GetKeyDown(KeyCode.Space))
        {
            GameObject.Find("UICanvas").transform.Find("ActionBtn").gameObject.SetActive(false);
            _dialogueManager.TalkStart();
            if (_dialogueManager.IsNextTalk)
            {
                List<string> tmp = new List<string>(Sentences_First);
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
                Sentences_First = tmp.ToArray();
                _dialogueManager.OnDialogue(Sentences_First);
                _dialogueManager.IsNextTalk = false;
            }
            else
            {
                _dialogueManager.OnDialogue(Sentences_First);
            }
            CanTalk = false;
        }
    }
}
