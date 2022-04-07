using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMove : MonoBehaviour
{
    private bool _canWarp;
    public string MapName;

    private FadeImage _fade;

    private void Awake()
    {
        _fade = GameObject.Find("Canvas").transform.Find("FadeImage").GetComponent<FadeImage>();
    }

    private void Start()
    {
        _fade.FadeOut();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.ActionBtnImage.gameObject.SetActive(true);
            Debug.Log("다음 맵 이동");
            _canWarp = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DialogueManager.Instance.ActionBtnImage.gameObject.SetActive(false);
        _canWarp = false;
    }

    private void Update()
    {
        if (_canWarp)
        {
            if (DialogueManager.Instance.ActionBtnImage.gameObject.activeSelf)
            {
                DialogueManager.Instance.ActionBtnImage.transform.position =
                    this.transform.position + new Vector3(-0.2f, 1f, 0);
            }
        }
    }
}
