using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interaction
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<BoxCollider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        StartInteract();
    }

    public override void StartInteract()
    {
        if (CanInteract)
        {
            ActionBtn.transform.position = this.GetComponent<BoxCollider>().transform.position + new Vector3(-0.7f, 1f, -0.5f);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                FindObjectOfType<SoundManager>().Play("Door",SoundType.Effect);
                this.GetComponent<Animator>().SetTrigger("OpenDoor");
                this.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
