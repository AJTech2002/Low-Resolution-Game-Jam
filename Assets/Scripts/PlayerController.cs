using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed;
    public Camera useCamera;
    private float speed;
    public float walkSpeed;
    public float sprintSpeed;
    [Header("SFX")]
    public AudioSource source;
    public List<AudioClip> footSteps;
    public float delay = 0.8f;
    private float timeDelay = 0f;
    public KeyManager keyManager;
    public bool canMove = true;
    public bool isWalking {
        get {
            return speed == walkSpeed;
        }
    }

    public bool isMoving = false;

    public Transform pointer;
    Vector3 mousePos;
    // Update is called once per frame
    
    private void Awake ()
    {
        speed = sprintSpeed;
        useCamera = GameObject.FindGameObjectWithTag("RenderCamera").GetComponent<Camera>();

        if (!useCamera) Debug.LogError("You dont have the World renderer my friend");
    }

    public float torchPickupDuration = 10;
    private float currentTorchPickup = 0f;
    private bool pickedUpTorch;
    private void PickupTorch ()
    {
        pickedUpTorch = true;
        currentTorchPickup = 0f;
        GetComponent<FOVMesh>().PickupTorch();
    }

    void Update()
    {
        if (pickedUpTorch)
        {
            currentTorchPickup += Time.deltaTime;
            if (currentTorchPickup >= torchPickupDuration)
            {
                pickedUpTorch = false;
                GetComponent<FOVMesh>().RemoveTorch();
            }
        }

        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 lookPos = useCamera.ScreenToWorldPoint(mousePos);
        
        lookPos.z = transform.position.z;


        if (Vector3.Distance(lookPos, transform.position) >= 0.2f)
        {
            lookPos = lookPos - useCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0),Camera.MonoOrStereoscopicEye.Mono);
            float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
       
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"),0);
        isMoving = movement.magnitude > 0 && canMove;
        //movement = pointer.TransformDirection(movement);
        movement *= Time.deltaTime * speed ;

        movement = Vector3.ClampMagnitude(movement, maxSpeed);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = walkSpeed;
        }
        
        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            speed = sprintSpeed;
        }

        if (speed == sprintSpeed && isMoving)
        {
            timeDelay += Time.deltaTime;
            if (timeDelay >= delay)
            {
                source.clip = footSteps[Random.Range(0, footSteps.Count)];
                source.Play();
                timeDelay = 0f;
            }
        }
        else {
            source.Stop();
        }
        
        if (canMove)
        transform.position += movement;

    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.CompareTag("Torch")) {
            PickupTorch();
            GameObject.Destroy(col.transform.gameObject);
        } else if (col.transform.CompareTag("Key")) {
            GameObject.FindObjectOfType<KeyManager>().PickupKey();
            GameObject.Destroy(col.transform.gameObject);
        } else if (col.transform.CompareTag("Door")) {
            if (!col.GetComponent<Door>().getIsOpen()) {
                col.GetComponent<Door>().OpenDoor();
            }
        }
    }
}
