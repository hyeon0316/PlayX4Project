using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpGauge : MonoBehaviour
{
    private Image healthPointBar;

    public float lerpSpeed;

    private float currentFill;

    public float myMaxValue { get; set; }

    private bool check;

    private float currentvalue;

    public float myCurrentValue
    {
        get
        {
            return currentvalue;
        }
        set
        {
            if (value > myMaxValue) currentvalue = myMaxValue;
            else if (value < 0) currentvalue  =0;
            else currentvalue = value;

            currentFill = currentvalue / myMaxValue;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        healthPointBar = GetComponent<Image>();

    }
    public void Initialize(float currentValue, float maxValue)
    {
        myMaxValue = maxValue;

        myCurrentValue = currentValue;
    }

    private void Update()
    {
                if (currentFill != healthPointBar.fillAmount)
        {
            //Mathf.Lerp(시작값, 끝값, 기준) -> 부드럽게 값을 변경 가능
            healthPointBar.fillAmount = Mathf.Lerp(healthPointBar.fillAmount, currentFill, Time.deltaTime * lerpSpeed);
        }
    }

}

    
