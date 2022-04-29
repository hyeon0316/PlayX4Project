using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_HpGauge : MonoBehaviour
{
    private Image healthPointBar;

    public static float timespan;

    public static float Damagetime;

    public float lerpSpeed;

    public static float currentFill;


    public float myMaxValue { get; set; }

    private bool check;

    public static float currentvalue;

    public float myCurrentValue
    {
        get
        {
            return currentvalue;
        }
        set
        {
            if (value > myMaxValue) currentvalue = myMaxValue;
            else if (value < 0) currentvalue = 0;
            else currentvalue = value;

            currentFill = currentvalue / myMaxValue;


        }
    }
    // Start is called before the first frame update
    void Start()
    {

        healthPointBar = GetComponent<Image>();
        currentFill = 1f;

    }
    public void Initialize(float currentValue, float maxValue)
    {
        myMaxValue = maxValue;

        myCurrentValue = currentValue;
    }

    public void Update()
    {

        if (currentFill < healthPointBar.fillAmount)
        {
            //Mathf.Lerp(시작값, 끝값, 기준) -> 부드럽게 값을 변경 가능
            healthPointBar.fillAmount = Mathf.Lerp(healthPointBar.fillAmount, currentFill, Time.deltaTime * lerpSpeed);
        }


        if (currentFill > healthPointBar.fillAmount)
        {
            //Mathf.Lerp(시작값, 끝값, 기준) -> 부드럽게 값을 변경 가능
            healthPointBar.fillAmount = Mathf.Lerp(currentFill, healthPointBar.fillAmount, Time.deltaTime * lerpSpeed);
        }
    }
}
