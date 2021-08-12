using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacking : MonoBehaviour
{
    public LayerMask mask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
        Debug.Log("test");
            RaycastHit2D hit = Physics2D.Raycast(transform.position,transform.forward,10,mask);
            Debug.DrawRay(transform.position, transform.forward, Color.green, 10, false);
            if (hit) {
                if (hit.transform != null) {
                    Debug.Log(hit.collider.gameObject.tag);
                }
            }
        }
    }
}
