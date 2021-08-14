using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepIndicator : MonoBehaviour
{
    List<Enemy> enemies = new List<Enemy>();
    [SerializeField] UnityEngine.UI.Image image;
    [SerializeField] UnityEngine.UI.Image seen;
    [SerializeField] UnityEngine.UI.Image searching;
    // Update is called once per frame


    private void Start ()
    {
        foreach (Enemy e in GameObject.FindObjectsOfType<Enemy>())
        {
            enemies.Add(e);
        }
    }
    void Update()
    {
        bool indicate3 = false;
        bool indicate2 = false;
        bool indicate = false;
        foreach (Enemy e in enemies)
        {
            if (e.canHearPlayer) { indicate = true; }
            if (e.canSeePlayer) { indicate2 = true; indicate = false; break; }

            if (e.currentEnemyState == EnemyState.Chasing)
            {
                indicate3 = true;
                
            }
        }
        
        image.enabled = indicate;
        seen.enabled = indicate2;

        if (!indicate && !indicate2)
        {
            searching.enabled = indicate3;
        }
        else {
            searching.enabled = false;
        }

        if (indicate3 || indicate2) 
        {
            GameObject.FindObjectOfType<BackgroundMusic>().PlayChase();
        } 
        else {
            GameObject.FindObjectOfType<BackgroundMusic>().PlayChill();
        }
    }
}
