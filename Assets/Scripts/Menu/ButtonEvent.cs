using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEvent : MonoBehaviour
{
    public GameObject Play;
    public GameObject Option;
    public GameObject Quit;
    public GameObject Exit;
    public GameObject Replay;
    public GameObject Quit_Option;
    public GameObject Option_Option;
    public GameObject Quit_Main;

    public void PB()
    {
        Play.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Play(B)", typeof(Sprite)) as Sprite;
    }
    public void PW()
    {
        Play.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Play(W)", typeof(Sprite)) as Sprite;
    }
    public void OB()
    {
        Option.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Option(B)", typeof(Sprite)) as Sprite;
    }
    public void OW()
    {
        Option.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Option(W)", typeof(Sprite)) as Sprite;
    }
    public void OB_Option()
    {
        Option_Option.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Option(B)", typeof(Sprite)) as Sprite;
    }
    public void OW_Option()
    {
        Option_Option.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Option(W)", typeof(Sprite)) as Sprite;
    }
    public void QB()
    {
        Quit.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Quit(B)", typeof(Sprite)) as Sprite;
    }
    public void QW()
    {
        Quit.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Quit(W)", typeof(Sprite)) as Sprite;
    }
    public void QB_Option()
    {
        Quit_Option.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Quit(B)", typeof(Sprite)) as Sprite;
    }
    public void QW_Option()
    {
        Quit_Option.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Quit(W)", typeof(Sprite)) as Sprite;
    }
    public void EB()
    {
        Exit.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Exit(B)", typeof(Sprite)) as Sprite;
    }
    public void EW()
    {
        Exit.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Exit(W)", typeof(Sprite)) as Sprite;
    }
    public void RB()
    {
        Replay.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Replay(B)", typeof(Sprite)) as Sprite;
    }
    public void RW()
    {
        Replay.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Replay(W)", typeof(Sprite)) as Sprite;
    }
    public void QB_Main()
    {
        Quit_Main.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Quit(B)", typeof(Sprite)) as Sprite;
    }
    public void QW_Main()
    {
        Quit_Main.GetComponent<Image>().sprite = Resources.Load("Raw and SpriteSheets/Menu Buttons/Large Buttons/Quit(W)", typeof(Sprite)) as Sprite;
    }
}
