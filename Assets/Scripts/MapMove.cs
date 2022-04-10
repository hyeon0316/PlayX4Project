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
    private CameraManager _camera;
    
    private void Awake()
    {
        _fade = GameObject.Find("Canvas").transform.Find("FadeImage").GetComponent<FadeImage>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _camera = GameObject.Find("Camera").GetComponent<CameraManager>();
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
            GameObject.Find("UICanvas").transform.Find("ActionBtn").transform.position = this.transform.position+ new Vector3(0,1.2f,0);
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject.Find("UICanvas").transform.Find("ActionBtn").gameObject.SetActive(false);
                _fade.FadeIn();
                _player.IsStop = true;
            }
            
            if (_fade.IsFade)
            {
                for (int i = 0; i < GameObject.Find("Colliders").transform.childCount; i++)
                {
                    GameObject.Find("Colliders").transform.GetChild(i).gameObject.SetActive(false);
                }
                GameObject.Find("Colliders").transform.Find(MapColliderName).gameObject.SetActive(true);
                _player.transform.position = NextMap.transform.position;
                
                _camera.BackgroudUpdate();
                
                _fade.FadeOut();
                _player.IsStop = false;
                _canWarp = false;
            }
        }
    }
}
