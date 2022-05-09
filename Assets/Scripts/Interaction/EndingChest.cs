using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingChest : Interaction
{
   private FadeImage _fade;
   protected override void Awake()
   {
      base.Awake();
      this.GetComponent<BoxCollider>().enabled = false;
      _fade = GameObject.Find("Canvas").transform.Find("FadeImage").GetComponent<FadeImage>();
   }

   private void Update()
   {
      StartInteract();
   }

   public override void StartInteract()
   {
      if (CanInteract)
      {
         if (GameObject.Find("PortalParent").GetComponentInChildren<Portal>().SummonEnemysTr.transform.childCount == 0)
         {
            ActionBtn.transform.position = this.transform.position + new Vector3(0, 2f, 0);
            if (Input.GetKeyDown(KeyCode.Space))
            {
               FindObjectOfType<Inventory>().DeleteMaterial();
               _fade.FadeIn();
               this.GetComponent<Animator>().SetTrigger("Open");
               CanInteract = false;
               FindObjectOfType<SoundManager>().Play("EndingBGM", SoundType.Bgm);
               Invoke("GoEnding", 1f);
            }
         }
      }
   }

   private void GoEnding()
   {
      SceneManager.LoadScene("Ending");
   }
}
