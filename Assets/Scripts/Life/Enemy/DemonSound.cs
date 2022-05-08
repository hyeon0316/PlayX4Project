using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonSound : MonoBehaviour
{
    public enum SoundName
    {
        DemonAttack,
        DemonHit,
        DemonDead,
    }

    public void SelectSound(SoundName name)
    {
        switch (name)
        {
           case SoundName.DemonAttack:
               FindObjectOfType<SoundManager>().Play("Enemy/Demon/DemonAttack",SoundType.Effect);
               break;
            case SoundName.DemonHit:
                int rand = Random.Range(0, 2);
                switch (rand)
                {
                    case 0:
                        FindObjectOfType<SoundManager>().Play("Enemy/Demon/DemonHit1",SoundType.Effect);
                        break;
                    case 1:
                        FindObjectOfType<SoundManager>().Play("Enemy/Demon/DemonHit2",SoundType.Effect);
                        break;
                }
                break;
            case SoundName.DemonDead:
                FindObjectOfType<SoundManager>().Play("Enemy/Demon/DemonDead",SoundType.Effect);
                break;

        }
    }
}
