using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interaction
{
    protected override void Awake()
    {
        base.Awake();
    }
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
                this.GetComponent<Animator>().SetTrigger("Open");
                CanInteract = false;
                ActionBtn.SetActive(false);
            }
        }
        else
        {
            
        }
    }

    
}
