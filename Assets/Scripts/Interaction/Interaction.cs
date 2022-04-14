using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
   public bool CanInteract;
   public GameObject ActionBtn;

   protected virtual void Awake()
   {
      ActionBtn = GameObject.Find("UICanvas").transform.Find("ActionBtn").gameObject;
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         Debug.Log("상호작용 가능");
         CanInteract = true;
         ActionBtn.SetActive(true);
      }
   }

   private void OnTriggerExit(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         Debug.Log("상호작용 불가능");
         CanInteract = false;
         ActionBtn.SetActive(false);
      }
   }
}
