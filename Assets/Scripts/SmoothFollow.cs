using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public float radiusOfControl;
    public float smoothSpeed;

    public Camera camera;

    public PlayerController controller;

    public Transform trackingObject;

    bool exceededControl = false;
    
    private void Awake ()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        
    }
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Cursor.lockState = CursorLockMode.None;

        if (trackingObject == null) trackingObject = GameObject.FindGameObjectWithTag("Player").transform;
        if (controller == null) controller = GameObject.FindObjectOfType<PlayerController>();

        if (trackingObject && controller)
        {
            Vector3 expectedPosition = new Vector3(trackingObject.position.x, trackingObject.position.y, transform.position.z);

            if (!exceededControl)
            {
                if (Vector3.Distance(expectedPosition, transform.position) >= radiusOfControl)
                {
                    exceededControl = true;
                }
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, expectedPosition, smoothSpeed * Time.deltaTime);

                if (Vector3.Distance(expectedPosition, transform.position) <= 0.1f)
                {
                    exceededControl = false;
                }
            }

            if (Input.GetKey(KeyCode.Space))
            {
                camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, 12f, 10 * Time.deltaTime);
                controller.canMove = false;
            }
            else
            {
                camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, 5f, 10 * Time.deltaTime);
                controller.canMove = true;
            }
        }
    }

}
