using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolDown : MonoBehaviour
{
    private Player _player;
    
    [Header("SKillA")]
    public KeyCode KeyA;
    public GameObject CoolDownA;
    private Image _fillImageA;
    private Text _coolTimeTextA;
    private bool _isCoolDownA;
    private float _timeA;
    
    [Header("SKillS")]
    public KeyCode KeyS;
    public GameObject CoolDownS;
    private Image _fillImageS;
    private Text _coolTimeTextS;
    private bool _isCoolDownS;
    private float _timeS;
    
    [Header("SKillD")]
    public KeyCode KeyD;
    public GameObject CoolDownD;
    private Image _fillImageD;
    private Text _coolTimeTextD;
    private bool _isCoolDownD;
    private float _timeD;
    
    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        
        _fillImageA = CoolDownA.transform.GetChild(0).GetComponent<Image>();
        _coolTimeTextA = CoolDownA.transform.GetChild(1).GetComponent<Text>();
        
        _fillImageS = CoolDownA.transform.GetChild(0).GetComponent<Image>();
        _coolTimeTextS = CoolDownA.transform.GetChild(1).GetComponent<Text>();
        
        _fillImageD = CoolDownA.transform.GetChild(0).GetComponent<Image>();
        _coolTimeTextD = CoolDownA.transform.GetChild(1).GetComponent<Text>();
    }

    private void LateUpdate()
    {
        UseA();
        UseS();
        UseD();
    }

    private void UseA()
    {
        if (Input.GetKeyDown(KeyA) && !_isCoolDownA)
        {
            Debug.Log(KeyA);
            CoolDownA.SetActive(true);
            _fillImageA.fillAmount = 1;
            _timeA = _player.CountTimeList[3];
            _isCoolDownA = true;
        }

        if (_isCoolDownA)
        {
            _timeA -= Time.deltaTime;
            _coolTimeTextA.text = $"{_timeA:N1}";

            if (_timeA <= 0)
            {
                _timeA = 0;
            }
        }
    }

    private void UseS()
    {
        if (Input.GetKeyDown(KeyS)&& !_isCoolDownS)
        {
            Debug.Log(KeyS);
            CoolDownS.SetActive(true);
            _fillImageS.fillAmount = 1;
            _isCoolDownS = true;

            if (_isCoolDownS)
            {
                _timeS -= Time.deltaTime;
                _coolTimeTextA.text = $"{_timeS:N1}";

                if (_timeS <= 0)
                    _timeS = 0;
            }
        }
    }

    private void UseD()
    {
        if (Input.GetKeyDown(KeyD)&& !_isCoolDownD)
        {
            Debug.Log(KeyD);
            CoolDownD.SetActive(true);
            _fillImageD.fillAmount = 1;
            _isCoolDownD = true;
        }

        if (_isCoolDownD)
        {
            _timeD -= Time.deltaTime;
            _coolTimeTextA.text = $"{_timeS:N1}";

            if (_timeD <= 0)
                _timeD = 0;
        }
    }
}
