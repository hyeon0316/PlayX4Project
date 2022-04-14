using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMove : Interaction
{
    public string MapColliderName;
    public Transform NextMap;

    private FadeImage _fade;
    private Player _player;
    private CameraManager _camera;
    
    protected override void Awake()
    {
        base.Awake();
        _fade = GameObject.Find("Canvas").transform.Find("FadeImage").GetComponent<FadeImage>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _camera = GameObject.Find("Camera").GetComponent<CameraManager>();
    }
    private void Start()
    {
        _fade.FadeOut();
    }
    
    private void Update()
    {
        StartInteract();
    }
    public override void StartInteract()
    {
        if (CanInteract)
        {
            ActionBtn.transform.position = this.transform.position+ new Vector3(0,1.2f,0);
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ActionBtn.SetActive(false);
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

                _camera.CameraMovetype = 0;
                _camera.BackgroudUpdate();
                _camera.transform.position += new Vector3(_camera.BackgroundImg.transform.position.x ,0,0);

                _camera.ChangeCameraType();
                _fade.FadeOut();
                _player.IsStop = false;
                CanInteract = false;
            }
        }
    }



   

}
