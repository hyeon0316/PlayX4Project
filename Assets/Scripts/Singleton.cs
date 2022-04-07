using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Destroy 여부 확인용
    private static bool _shuttingDown = false;
    private static object _lock = new object();
    
    private static T _instance;
    public static T Instance
    {
        get
        {
            // 게임 종료 시 Object 보다 싱글톤의 OnDestroy 가 먼저 실행 될 수도 있다. 
            // 해당 싱글톤을 gameObject.Ondestory() 에서는 사용하지 않거나 사용한다면 null 체크를 해주자
            if (_shuttingDown)
            {
                Debug.Log($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                return null;
            }

            lock (_lock) //Thread Safe
            {
                if (_instance == null)
                {
                    // 인스턴스 존재 여부 확인
                    _instance = (T) FindObjectOfType(typeof(T));

                    // 아직 생성되지 않았다면 인스턴스 생성
                    if (_instance == null)
                    {
                        // 새로운 게임오브젝트를 만들어서 싱글톤 Attach
                        var singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
                        
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return _instance;
            }
        }
    }

    private void OnApplicationQuit()
    {
        _shuttingDown = true;
    }

    private void OnDestroy()
    {
        _shuttingDown = true;
    }
}