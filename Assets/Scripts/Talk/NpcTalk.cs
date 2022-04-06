using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcTalk : MonoBehaviour
{
    public string[] Sentences;
    public bool CanTalk = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("대화 가능");
            CanTalk = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("대화 불가능");
        CanTalk = false;
    }

    private void Update()
    {
        if (CanTalk && Input.GetKeyDown(KeyCode.Space))
        {
            //todo: 플레이어 이동 제한하기

            TalkStart();
        }
    }

    private void TalkStart()
    {
        Debug.Log("대화");
        DialogueManager.Instance.TalkStart();
        DialogueManager.Instance.OnDialogue(Sentences);
        CanTalk = false;
    }
}
