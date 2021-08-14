using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public int keys = 0;
    public GameObject mainCamera;
    public GameObject keyIndicator;
    public GameObject[] keyIndicators = new GameObject[4];
    public float xOffset = 0f;
    public float yOffset = 0f;
    public float betweenWidth = 1f;

    private void Start()
    {
        mainCamera = Camera.main.gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        if (mainCamera)
        {
            for (int i = 0; i < 4; i++)
            {
                float xValue = mainCamera.transform.position.x + xOffset + i * betweenWidth;
                Vector3 position = new Vector3(xValue, mainCamera.transform.position.y + yOffset, 0);
                if (keyIndicators[i] != null)
                {
                    keyIndicators[i].transform.position = position;
                }
            }
        }
        else mainCamera = Camera.main.gameObject;
    }

    public void PickupKey() {
        if (keys < 4) {
            Vector3 position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);
            keyIndicators[keys] = Instantiate(keyIndicator, position, Quaternion.identity);
            keys++;
        }
    }

    public int NumberOfKeys() {
        return keys;
    }

    public bool UseKey() {
        if (keys > 0) {
            GameObject.Destroy(keyIndicators[keys - 1]);
            keyIndicators[keys - 1] = null;
            keys--;
            return true;
        } else {
            return false;
        }
    }
}
