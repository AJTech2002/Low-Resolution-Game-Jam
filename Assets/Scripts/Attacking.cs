using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacking : MonoBehaviour
{
    public LayerMask mask;
    public Animation animation;

    public Sound enemyKill;
    public Sound deathSound;
    public Sound swing;

    public float delay;
    private float del;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        del += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && del >= delay) {
        
            RaycastHit2D hit = Physics2D.Raycast(transform.position,transform.right,2,mask);
            Debug.DrawRay(transform.position, transform.right, Color.green, 2, false);
            animation.Play();
            del = 0f;
            if (hit) {
                if (hit.transform != null) {
                    if (hit.transform.CompareTag("Enemy") && Vector3.Dot(transform.right,hit.transform.right) > 0.7f)
                    {
                        enemyKill.PlayAt(hit.transform.position, 1f, 0f);
                        GameObject.Destroy(hit.transform.gameObject);
                    }
                    else if (hit.transform.CompareTag("King"))
                    {
                        deathSound.PlayAt(hit.transform.position, 1f, 0f);
                        GameObject.Destroy(hit.transform.gameObject);
                        GameManager.ProgressScene();
                    }
                    else {
                        swing.PlayAt(transform.position, 0.3f, 0.01f);
                    }
                }
            }
            else {
                        swing.PlayAt(transform.position, 0.3f, 0.01f);
                    }
        }
    }
}
