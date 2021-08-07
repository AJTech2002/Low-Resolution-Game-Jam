using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public float radiusOfControl;
    public float smoothSpeed;
 
    public Transform trackingObject;

    bool exceededControl = false;
    
    private void Awake ()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void Update()
    {
        Vector3 expectedPosition = new Vector3(trackingObject.position.x, trackingObject.position.y, transform.position.z);

        if (!exceededControl)
        {
            if (Vector3.Distance(expectedPosition, transform.position) >= radiusOfControl)
            {
                exceededControl = true;
            }
        }
        else {
            transform.position = Vector3.Lerp(transform.position, expectedPosition, smoothSpeed*Time.deltaTime);

            if (Vector3.Distance(expectedPosition, transform.position) <= 0.1f)
            {
                exceededControl = false;
            }
        }

    }

}
