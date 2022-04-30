﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillAmount_D : MonoBehaviour
{
    public Image skillFilter;
    public Text coolTimeCounter; //남은 쿨타임을 표시할 텍스트

    public float coolTime;

    private float currentCoolTime; //남은 쿨타임을 추적 할 변수
    public GameObject time;

    private bool canUseSkill_D = true; //스킬을 사용할 수 있는지 확인하는 변수
  

    public GameObject Skill_Z;
    public Image Skill_D_Image;

   
    Image thisImg;
    void start()
    {
       
        Skill_D_Image = Skill_Z.GetComponent<Image>();
        skillFilter.fillAmount = 0; 
        thisImg = GetComponent<Image>();
    }

    public void UseSkill_D()
    {
        coolTime = FindObjectOfType<Player>().CountTimeList[4];
        if (canUseSkill_D)
        {
            Debug.Log("Use Skill");
            skillFilter.fillAmount = 1; //스킬 버튼을 가림
            StartCoroutine("Cooltime");

            currentCoolTime = coolTime;
            coolTimeCounter.text = "" + currentCoolTime;

            StartCoroutine("CoolTimeCounter");

            canUseSkill_D = false; //스킬을 사용하면 사용할 수 없는 상태로 바꿈
            Skill_D_Image.sprite = Resources.Load<Sprite>("GUI_Parts/Icons/skill/sword4(B)")
                as Sprite;
            time.SetActive(true);     
        }
        else
        {
            Debug.Log("아직 스킬을 사용할 수 없습니다.");
        }
    }

    private void Update()
    {
        if (canUseSkill_D == true)
        {
            Skill_D_Image.sprite = Resources.Load<Sprite>("GUI_Parts/Icons/skill/sword1")
                as Sprite;
        }
    }

    IEnumerator Cooltime()
    {
        while (skillFilter.fillAmount > 0)
        {
            skillFilter.fillAmount -= 1 * Time.smoothDeltaTime / coolTime;

            yield return null;
        }

        canUseSkill_D = true; //스킬 쿨타임이 끝나면 스킬을 사용할 수 있는 상태로 바꿈
        yield break;
    }
    IEnumerator CoolTimeCounter()
    {
        while (currentCoolTime > 0)
        {
            yield return new WaitForSeconds(1.0f);

            currentCoolTime -= 1.0f;
            coolTimeCounter.text = "" + currentCoolTime;
          
        }
        if (currentCoolTime == 0)
        {
            time.SetActive(false);

        }


            yield break;
    }

    //-----------------------------------------------------------------------------


}


