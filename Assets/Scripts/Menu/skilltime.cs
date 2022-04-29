using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class skilltime : MonoBehaviour
{
    public Image black;
    private float maxCooldown = 2f;
    private float currentCooldown = 2f;
    public GameObject Imageblack;


    public void SetMaxCooldown(in float value)
    {
        maxCooldown = value;
        UpdateFiilAmount();
    }

    public void SetCurrentCooldown(in float value)
    {
        currentCooldown = value;
        UpdateFiilAmount();
    }

    private void UpdateFiilAmount()
    {


        black.fillAmount = currentCooldown / maxCooldown;

    }
    private void Update()
    {
        SetCurrentCooldown(currentCooldown - Time.deltaTime);


        if (currentCooldown < 0f)
            currentCooldown = maxCooldown;

        if (black.fillAmount <= 0)
        {
            Imageblack.SetActive(false);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Imageblack.SetActive(true);
        }
    }
}