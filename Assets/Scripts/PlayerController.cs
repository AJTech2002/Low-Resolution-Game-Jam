using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed;
    public Camera useCamera;
    public float speed;
    public Transform pointer;
    Vector3 mousePos;
    // Update is called once per frame
    
    
    void Update()
    {

        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 lookPos = useCamera.ScreenToWorldPoint(mousePos);
        
        lookPos.z = transform.position.z;


        if (Vector3.Distance(lookPos, transform.position) >= 0.2f)
        {
            Debug.DrawLine(transform.position, lookPos, Color.red, 0.1f);
            lookPos = lookPos - transform.position;
            float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        Vector3 movement = new Vector3(0,-Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        movement = pointer.TransformDirection(movement);
        movement *= Time.deltaTime * speed ;

        movement = Vector3.ClampMagnitude(movement, maxSpeed);


        transform.position += movement;

    }
}
