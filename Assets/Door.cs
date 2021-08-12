using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool isOpen = false;

    public void OpenDoor() {

        if (GameObject.FindObjectOfType<KeyManager>().NumberOfKeys() > 0)
        {
            if (!isOpen)
            {
                gameObject.GetComponent<Animation>().Play();
                isOpen = true;
                GameObject.FindObjectOfType<KeyManager>().UseKey();
            }
        }
    }

    public bool getIsOpen() {
        return isOpen;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player")) OpenDoor();
    }
}
