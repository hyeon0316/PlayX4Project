using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialTrigger : MonoBehaviour
{
   private Tutorial _tutorial;

   private void Awake()
   {
      _tutorial = FindObjectOfType<Tutorial>();
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         for (int i = 0; i < _tutorial.Keys.Length; i++)
         {
            _tutorial.Keys[i].SetActive(false);
         }
         
         if (this.gameObject.name.Equals("CBtnOn"))
         {
            _tutorial.Keys[(int)KeyIcons.Jump].SetActive(true);
         }
         else if (this.gameObject.name.Equals("ShiftBtnOn"))
         {
            _tutorial.Keys[(int)KeyIcons.Shift].SetActive(true);
         }
         else if (this.gameObject.name.Equals("WallJumpOn"))
         {
            _tutorial.ManualWindow.SetActive(true);
         }
         else if (this.gameObject.name.Equals("CombatOn"))
         {
            _tutorial.PlayerUICanvas.SetActive(true);
            Instantiate(_tutorial.PracticeEnemy, this.transform.position, this.transform.rotation);
            FindObjectOfType<Player>().HP -= 20f;
            FindObjectOfType<Inventory>().AddUsed(_tutorial.DropItem, 1);
         }
         else if (this.gameObject.name.Equals("GameStart"))
         {
            GameObject.Find("Canvas").transform.Find("FadeImage").GetComponent<FadeImage>().FadeIn();
            Invoke("DelayLoadScene", 2f);
         }
         this.gameObject.SetActive(false);
      }
   }

   private void DelayLoadScene()
   {
      FindObjectOfType<SoundManager>().Play("TownBGM", SoundType.Bgm);
      SceneManager.LoadScene("Town");
   }
   
}
