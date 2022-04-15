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
        if (CanInteract)
        {
            ActionBtn.transform.position = this.transform.position + new Vector3(0f, 1f, 0.5f);
        }
      

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
            GameObject.Find("Camera").GetComponent<CameraManager>().Player = this.gameObject;
            ActionBtn.SetActive(false);
            _dialogueManager.TalkStart();
            if (_dialogueManager.IsNextTalk)
            {
                List<string> tmp = new List<string>(Sentences);
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

                Sentences = tmp.ToArray();
                _dialogueManager.OnDialogue(Sentences);
                _dialogueManager.IsNextTalk = false;
            }
            else
            {
                _dialogueManager.OnDialogue(Sentences);
            }

            CanInteract = false;
        }
    }
}

   

