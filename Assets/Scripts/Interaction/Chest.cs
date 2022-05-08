using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interaction
{
    public Item DropItem;
    public int ItemCount;

    private GameObject _dropItem;
    private Transform _dropPos;

    protected override void Awake()
    {
        base.Awake();
        _dropItem = Resources.Load<GameObject>("Prefabs/DropItem");
        _dropPos = this.transform.GetChild(0).GetComponent<Transform>();
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
                FindObjectOfType<SoundManager>().Play("Object/Chest",SoundType.Effect);
                CanInteract = false;
                ActionBtn.SetActive(false);
                this.GetComponent<BoxCollider>().enabled = false;
                this.GetComponent<Animator>().SetTrigger("Open");
                FindObjectOfType<Inventory>().AddUsed(DropItem, ItemCount);

                for (int i = 0; i < ItemCount; i++)
                {
                    Instantiate(_dropItem, _dropPos.position, _dropPos.rotation);
                }

                if(FindObjectOfType<GameManager>().Walf[3].activeSelf)
                {
                    FindObjectOfType<Inventory>().AddMaterial("SecretKey");
                    FindObjectOfType<Door>().CanOpen = true;
                }

                if (FindObjectOfType<GameManager>().EnemyPos[1].transform.parent.gameObject.activeSelf)
                {
                    FindObjectOfType<Player>().transform.position = GameObject.Find("RevivalPos").transform.position;
                    FindObjectOfType<CameraManager>().transform.position = FindObjectOfType<Player>().transform.position + Vector3.up;
                }

                //y - 1.8 , z - 4.5

            }
        }
    }

   
    
}
