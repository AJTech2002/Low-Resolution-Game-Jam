using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float walkSpeed;
    public float sprintSpeed;
    public float jumpHeight;

    [Space]
    public Rigidbody rb;
    public Transform groundChecker;
    public LayerMask ground;
    Vector3 movement;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        movement = new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical"));
        RaycastHit hit;
        if (Physics.Raycast(groundChecker.position, Vector3.down, out hit, .1f, ground))
        {


            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
            }

        }
    }

    void FixedUpdate()
    {
        moveCharacter(movement);
    }

    void moveCharacter(Vector3 direction)
    {


        direction = rb.rotation * direction;

       

        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.MovePosition(rb.position + direction * sprintSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.MovePosition(rb.position + direction * walkSpeed * Time.fixedDeltaTime);
        }
    }

 
}
