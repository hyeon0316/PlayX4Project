using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interaction
{
    public Item DropItem;
    public int ItemCount;
  
    private void Update()
    {
        StartInteract();
    }
    
    public override void StartInteract()
    {
        if (CanInteract)
        {
            ActionBtn.SetActive(true);
            ActionBtn.transform.position = this.transform.position + new Vector3(0f, 1.5f, 0.5f);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.GetComponent<BoxCollider>().enabled = false;
                this.GetComponent<Animator>().SetTrigger("Open");
                FindObjectOfType<Inventory>().AddUsed(DropItem, ItemCount);
                if(FindObjectOfType<GameManager>().Walf[3].activeSelf)
                {
                    FindObjectOfType<Inventory>().AddMaterial("SecretKey");
                    FindObjectOfType<Door>().GetComponent<BoxCollider>().enabled = true;
                }
            }
        }
    }

    
}
