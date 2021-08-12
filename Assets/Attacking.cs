using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacking : MonoBehaviour
{
    public LayerMask mask;
    public Animation animation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
        
            RaycastHit2D hit = Physics2D.Raycast(transform.position,transform.right,2,mask);
            Debug.DrawRay(transform.position, transform.right, Color.green, 2, false);
            animation.Play();
            if (hit) {
                if (hit.transform != null) {
                    if (hit.transform.CompareTag("Enemy") && Vector3.Dot(transform.right,hit.transform.right) > 0.7f)
                    {
                        GameObject.Destroy(hit.transform.gameObject);
                    }
                }
            }
        }
    }
}
