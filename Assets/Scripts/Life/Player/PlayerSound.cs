using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum SoundName
{
    TownFoot,
    XAttack,
    PlayerHit,
    PlayerJump,
    PlayerRoll,
    FlyAttack,
    ZAttack1,
    ZAttack2,
    DashAttack,
    GunSound,
}
public class PlayerSound : MonoBehaviour
{
    public void SelectSound(SoundName name)
    {
        switch (name)
        {
            case SoundName.TownFoot:
                if(SceneManager.GetActiveScene().name.Equals("Town"))
                    FindObjectOfType<SoundManager>().Play("TownFoot",SoundType.Effect);
                else if(SceneManager.GetActiveScene().name.Equals("Dungeon"))
                    FindObjectOfType<SoundManager>().Play("DungeonFoot",SoundType.Effect);
                break;
            case SoundName.XAttack:
                FindObjectOfType<SoundManager>().Play("XAttack",SoundType.Effect);
                break;
            case SoundName.PlayerHit:
                int rand = Random.Range(0, 2);
                switch (rand)
                {
                    case 0:
                        FindObjectOfType<SoundManager>().Play("PlayerHit1",SoundType.Effect);
                        break;
                    case 1:
                        FindObjectOfType<SoundManager>().Play("PlayerHit2",SoundType.Effect);
                        break;
                }
                break;
            case SoundName.PlayerJump:
                FindObjectOfType<SoundManager>().Play("PlayerJump",SoundType.Effect);
                break;
            case SoundName.PlayerRoll:
                FindObjectOfType<SoundManager>().Play("PlayerRoll",SoundType.Effect);
                break;
            case SoundName.FlyAttack:
                FindObjectOfType<SoundManager>().Play("FlyAttack",SoundType.Effect);
                break;
            case SoundName.ZAttack1:
                FindObjectOfType<SoundManager>().Play("ZAttack1",SoundType.Effect);
                break;
            case SoundName.ZAttack2:
                FindObjectOfType<SoundManager>().Play("ZAttack2",SoundType.Effect);
                break;
            case SoundName.DashAttack:
                FindObjectOfType<SoundManager>().Play("DashAttack",SoundType.Effect);
                break;
            case SoundName.GunSound:
                FindObjectOfType<SoundManager>().Play("GunSound",SoundType.Effect);
                break;
        }
    }
}
