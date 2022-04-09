using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMove : MonoBehaviour
{
    private bool _canWarp;
    public string MapColliderName;
    public Transform NextMap;

    private FadeImage _fade;
    private Player _player;
    
    private void Awake()
    {
        _fade = GameObject.Find("Canvas").transform.Find("FadeImage").GetComponent<FadeImage>();
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    private void Start()
    {
        _fade.FadeOut();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("다음 맵 이동");
            _canWarp = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _canWarp = false;
    }

    private void Update()
    {
        if (_canWarp)
        {
            GameObject.Find("UICanvas").transform.Find("ActionBtn").gameObject.SetActive(true);
            GameObject.Find("UICanvas").transform.Find("ActionBtn").transform.position = this.transform.position + new Vector3(-0.2f, 1f, 0);
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject.Find("UICanvas").transform.Find("ActionBtn").gameObject.SetActive(false);
                _fade.FadeIn();
                _player.IsStop = true;
            }
            
            if (_fade.FadeCount >= 1f)
            {
                _player.transform.position = NextMap.transform.position;
                _fade.FadeOut();
                _player.IsStop = false;
                _canWarp = false;
            }
        }
        else
        {
            GameObject.Find("UICanvas").transform.Find("ActionBtn").gameObject.SetActive(false);
        }
    }
}
