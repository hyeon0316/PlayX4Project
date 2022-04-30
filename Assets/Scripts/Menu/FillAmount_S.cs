using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillAmount_S : MonoBehaviour
{
    public GameObject Player;
    public Image skillFilter;
    public Text coolTimeCounter; //남은 쿨타임을 표시할 텍스트

    public float coolTime;

    private float currentCoolTime; //남은 쿨타임을 추적 할 변수
    public GameObject time;

    private bool canUseSkill_S = true; //스킬을 사용할 수 있는지 확인하는 변수

    
    public GameObject Skill_S;
    public Image Skill_S_Image;


    Image thisImg;
    void start()
    {
        

      


       Skill_S_Image = Skill_S.GetComponent<Image>();
        skillFilter.fillAmount = 0; 
        thisImg = GetComponent<Image>();
    }

    public void UseSkill_S()
    {
        if (canUseSkill_S)
        {
            Debug.Log("Use Skill");
            skillFilter.fillAmount = 1; //스킬 버튼을 가림
            StartCoroutine("Cooltime");

            currentCoolTime = coolTime;
            coolTimeCounter.text = "" + currentCoolTime.ToString("N1");

            StartCoroutine("CoolTimeCounter");

            canUseSkill_S = false; //스킬을 사용하면 사용할 수 없는 상태로 바꿈
            Skill_S_Image.sprite = Resources.Load<Sprite>("GUI_Parts/Icons/skill/sword3(B)")
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
        Player call = GameObject.Find("Player").GetComponent<Player>();
        call.CountTimeList[0] += coolTime;
        Player.GetComponent<Player>().Skill();
        if (canUseSkill_S == true)
        {
            Skill_S_Image.sprite = Resources.Load<Sprite>("GUI_Parts/Icons/skill/sword3")
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

        canUseSkill_S = true; //스킬 쿨타임이 끝나면 스킬을 사용할 수 있는 상태로 바꿈
        yield break;
    }
    IEnumerator CoolTimeCounter()
    {
        while (currentCoolTime > 0)
        {
            yield return new WaitForSeconds(1.0f);

            currentCoolTime -= 1.0f;
            coolTimeCounter.text = "" + currentCoolTime.ToString("N1");
          
        }
        if (currentCoolTime == 0)
        {
            time.SetActive(false);

        }


            yield break;
    }

    //-----------------------------------------------------------------------------


}


