using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : Singleton<AnimationManager>
{
    /// <summary>
    /// 1번째 인자값 애니메이터의 int변수을 value 로 변경하는 함수
    /// </summary>
    /// <param name="animator">변경할 애니메이터</param>
    /// <param name="name">변경한 변수의 이름</param>
    /// <param name="value">변경할 값</param>
    public void AnimatorControllInt(Animator animator,string name ,int value )
    {
        animator.SetInteger(name, value);
    }
    /// <summary>
    /// 1번째 인자값 애니메이터의 float 변수를 value 로 변경하는 함수
    /// </summary>
    /// <param name="animator">변경할 애니메이터</param>
    /// <param name="name">변경할 변수의 이름</param>
    /// <param name="value">변경할 값</param>
    public void AnimatorControllFloat(Animator animator, string name, float value)
    {
        animator.SetFloat(name, value);
    }
    /// <summary>
    /// 1번째 인자값 애니메이터의 bool변수을 value 로 변경하는 함수
    /// </summary>
    /// <param name="animator">변경할 애니메이터</param>
    /// <param name="name">변경한 변수의 이름</param>
    /// <param name="value">변경할 값</param>
    public void AnimatorControllBool(Animator animator, string name, bool value)
    {
        animator.SetBool(name, value);
    }
    /// <summary>
    /// 1번째 인자의 애니메이터의 trigger 를 작동하는 함수
    /// </summary>
    /// <param name="animator">실행할 트리거가 들어가 있는 애니메이터</param>
    /// <param name="name">실행할 트리거 이름</param>
    public void AnimatorControllTrigger(Animator animator, string name)
    {
        animator.SetTrigger(name);
    }

    /// <summary>
    /// 1번째 애니메이터가 플레이하고 있는 애니메이션이 2번째 인자인지, 2번째 인자일때 애니메이션이 끝까지 실행되였는지 확인
    /// </summary>
    /// <param name="animator">확인할 애니메이터</param>
    /// <param name="playAnimation">확인할 애니메이션 이름</param>
    /// <returns>만약 실행되고 있는 애니메이션이 2번째 인자의 이름과 같고, 끝까지 실행됬을때 true 를 반환</returns>
    public bool AnimatorPlayEnd(Animator animator, string playAnimation)
    {
        AnimatorStateInfo AniStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!AniStateInfo.IsName(playAnimation)) {//현재 실행중인 애니메이션이 2번째 인자값의 내용이 아니라면 flase 를 반환
            return false;
        }
        //현재 플레이중인 애니메이션 이름이 2번째 인자값 이름일때
        else
        {
            //애니메이션이 끝났다면  true 아니면 false 를 반환
            if(AniStateInfo.normalizedTime >= 1f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
