using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{

    public Player Player;

    private bool _isladder;

    private bool _isPlayeruse;

    

    private void Awake()
    {
        Player = GameObject.Find("Player").GetComponent<Player>();
        _isPlayeruse = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (_isPlayeruse)
        {
            if (other.CompareTag("Player")) { 
            Player.ChangeLadder(this.gameObject, false);
            _isladder = false;
            _isPlayeruse = false;
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {

        // GameObject.Find("UICanvas").transform.Find("ActionBtn").gameObject.SetActive(true);

        //플레이어가 사용하고 있지 않을때
        if (!_isPlayeruse) { 
            if (other.CompareTag("Player")) {
                Debug.Log("충돌 확인");
              
           
                _isladder = true;
            }
        }
        else//플레이어가 사용중일때
        {
            if (other.CompareTag("Player"))
            {
                _isladder = true;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("충돌 탈출 확인");
            //   GameObject.Find("UICanvas").transform.Find("ActionBtn").gameObject.SetActive(false);
            _isladder = false;
        }
    }

    public void Update()
    {
        if (!_isPlayeruse) { 
            if (_isladder)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Player.ChangeLadder(this.gameObject, true);
                    _isladder = false;
                    _isPlayeruse = true;
                }
            }
        }
        else
        {
            if (_isladder)
            {
                
                   
                
            }
        }
    }


}
