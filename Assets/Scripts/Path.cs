using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{

    public List<Vector3> path = new List<Vector3>();

    private void Awake ()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            path.Add(transform.GetChild(i).transform.position);
        }
    }

    private void OnDrawGizmos ()
    {
        
        for (int i = 0; i < transform.childCount; i++)
        {Gizmos.color = Color.yellow;
            if (i == 0) continue;
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i - 1).position);
            Gizmos.color = Color.blue;
            if (i == transform.childCount-1)  Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(0).position);
        }
    }

    public bool ValidIndex (int i)
    {
        if ( i < 0) return false;
        if (i > path.Count-1) return false;

        return true;
    }

    public int NextPoint (int i)
    {
        int c = i + 1;

        if (!ValidIndex(c))
            c = 0;

        return c;
    }

    public bool ReachedIndex (int i, Vector3 pos)
    {
        if (!ValidIndex(i)) return false;


        if (Vector3.Distance(pos, path[i]) <= 0.9f)
        {
            return true;
        }

        return false;

    }
}
