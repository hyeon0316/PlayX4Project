using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcTalk : Interaction
{
    public string[] Sentences;
    private DialogueManager _dialogueManager;

    protected override void Awake()
    {
        base.Awake();
        _dialogueManager = GameObject.Find("Canvas").GetComponent<DialogueManager>();
    }

    private void Update()
    {
        StartInteract();
    }

    public override void StartInteract()
    {
        if (CanInteract && Input.GetKeyDown(KeyCode.Space))
        {
            if (transform.position.x > GameObject.Find("Player").transform.position.x)
            {
                GetComponentInChildren<SpriteRenderer>().flipX = true;
            }
            else
            {
                GetComponentInChildren<SpriteRenderer>().flipX = false;
            }
            
            _dialogueManager.Npc = this.GetComponent<NpcTalk>();
            _dialogueManager.TalkPanel.transform.position = this.transform.position + new Vector3(0.8f, 1.2f, 0.5f);

            FindObjectOfType<Player>().PlayerAnim.SetBool("IsRun", false);
            FindObjectOfType<CameraManager>().Target = this.gameObject;
            ActionBtn.SetActive(false);
            _dialogueManager.TalkStart();
            _dialogueManager.OnDialogue(Sentences);

            CanInteract = false;
        }
    }
}

   

