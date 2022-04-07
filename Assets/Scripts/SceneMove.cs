using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    private bool _canWarp;
    private string _sceneName = "Dungeon";

    private FadeImage _fade;

    private void Awake()
    {
        _fade = GameObject.Find("Canvas").transform.Find("FadeImage").GetComponent<FadeImage>();
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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject.Find("UICanvas").transform.Find("ActionBtn").gameObject.SetActive(false);
                _fade.FadeIn();
            }
            
           if(_fade.FadeCount >= 1f)
            {
                SceneManager.LoadScene(_sceneName);
                _canWarp = false;
            }
        }

       
    }
}
