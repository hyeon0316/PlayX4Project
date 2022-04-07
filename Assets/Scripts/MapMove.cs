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
            GameObject.Find("UICanvas").transform.Find("ActionBtn").gameObject.SetActive(true);
            Debug.Log("다음 맵 이동");
            _canWarp = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject.Find("UICanvas").transform.Find("ActionBtn").gameObject.SetActive(false);
        _canWarp = false;
    }

    private void Update()
    {
        if (_canWarp)
        {
            if (GameObject.Find("UICanvas").transform.Find("ActionBtn").gameObject.activeSelf)
            {
                GameObject.Find("UICanvas").transform.Find("ActionBtn").transform.position =
                    this.transform.position + new Vector3(-0.2f, 1f, 0);
            }
        }
    }
}
