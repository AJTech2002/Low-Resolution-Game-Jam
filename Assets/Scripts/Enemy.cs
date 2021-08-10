

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public enum EnemyState
{
    Patrolling,
    Chasing,
    RandomSearching
};



public class Enemy : MonoBehaviour
{
    [Header("SFX")]
    public List<AudioClip> footSteps;
    public float minFootstepDelay = 0.8f;
    public float maxFootstepDelay = 1.2f;
    public AudioSource source;

    [Header("AI")]
    public Path enemyPath;
    public NavMeshAgent agent;
    private Transform playerRef;
    public EnemyState currentEnemyState = EnemyState.Patrolling;

    static string PATROL = "BeginPatrol";

    private void Start ()
    {
        StartCoroutine("Footstep");
        StartCoroutine(PATROL);
        playerRef = GameObject.FindGameObjectWithTag("Player").transform;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update ()
    {
        

        Vector3 dif = playerRef.position - transform.position;

        float spread = -Vector3.Dot(Vector3.right, dif.normalized);

        source.panStereo = spread;
        source.volume = 1-(Mathf.Clamp(dif.magnitude-source.minDistance, 0, source.maxDistance)/(source.maxDistance-source.minDistance));
        
        Vector3 mousePos = new Vector3(playerRef.position.x, playerRef.position.y, 0);
        
        mousePos.z = transform.position.z;


        if (Vector3.Distance(mousePos, transform.position) >= 0.2f)
        {
            mousePos = mousePos - transform.position;
            float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

    }

    private void LookAt (Vector3 pos)
    {
        Vector3 mousePos = pos - transform.position;
        mousePos.z = transform.position.z;
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    IEnumerator BeginPatrol ()
    {
        int pathIndex = 0;
        Vector3 newSteeringTarget = Vector3.zero;

        while (currentEnemyState == EnemyState.Patrolling)
        {
            if (enemyPath != null && enemyPath.path.Count > 0)
            {
                yield return new WaitForEndOfFrame();
                
                
                if (agent.path.corners.Length > 0)
                    newSteeringTarget = Vector3.Lerp(newSteeringTarget,agent.steeringTarget,5*Time.deltaTime);
                else
                    newSteeringTarget = Vector3.Lerp(newSteeringTarget,enemyPath.path[pathIndex],5*Time.deltaTime);

                LookAt(newSteeringTarget);

                agent.SetDestination(enemyPath.path[pathIndex]);
                
                if (enemyPath.ReachedIndex(pathIndex, transform.position))
                {
                    pathIndex = enemyPath.NextPoint(pathIndex);
                }
            }
        }
    }

    private IEnumerator Footstep ()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minFootstepDelay, maxFootstepDelay));
            source.clip = footSteps[Random.Range(0, footSteps.Count)];
            source.Play();
        }
    }

}
