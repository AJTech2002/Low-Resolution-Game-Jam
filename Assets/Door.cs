using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool isOpen = false;

    public void OpenDoor() {
        if (!isOpen) {
            gameObject.GetComponent<Animation>().Play();
            isOpen = true;
        }
    }

    public bool getIsOpen() {
        return isOpen;
    }
}
