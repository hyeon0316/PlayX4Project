﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcTalk : MonoBehaviour
{
    public string[] Sentences_First;
    public bool CanTalk = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.ActionBtnImage.gameObject.SetActive(true);
            Debug.Log("대화 가능");
            CanTalk = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DialogueManager.Instance.ActionBtnImage.gameObject.SetActive(false);
        Debug.Log("대화 불가능");
        CanTalk = false;
    }

    private void Update()
    {
        TalkStart();
    }

    private void TalkStart()
    {
        if (CanTalk && Input.GetKeyDown(KeyCode.Space))
        {
            DialogueManager.Instance.ActionBtnImage.gameObject.SetActive(false);
            DialogueManager.Instance.TalkStart();

            if (DialogueManager.Instance.IsNextTalk)
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
                DialogueManager.Instance.OnDialogue(Sentences_First);
                DialogueManager.Instance.IsNextTalk = false;
            }
            else
            {
                DialogueManager.Instance.OnDialogue(Sentences_First);
            }
            
            CanTalk = false;
        }
    }
}
