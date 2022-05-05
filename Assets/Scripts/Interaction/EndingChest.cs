using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingChest : Interaction
{
   private FadeImage _fade;
   protected override void Awake()
   {
      base.Awake();
      this.GetComponent<BoxCollider>().enabled = false;
      _fade = FindObjectOfType<FadeImage>();
   }

   private void Update()
   {
      StartInteract();
   }

   public override void StartInteract()
   {
      if (CanInteract)
      {
         if (Input.GetKeyDown(KeyCode.Space))
         {
            FindObjectOfType<Inventory>().DeleteMaterial();
            _fade.FadeIn();
            this.GetComponent<Animator>().SetTrigger("Open");
            CanInteract = false;
         }
      }

      if (_fade.IsFade)
      {
         //todo: 엔딩씬 이동
      }
      
   }
}
