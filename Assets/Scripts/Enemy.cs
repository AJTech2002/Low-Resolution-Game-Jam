

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
    public LayerMask seeMask;

    [Header("SFX")]
    public List<AudioClip> footSteps;
    public float minFootstepDelay = 0.8f;
    public float maxFootstepDelay = 1.2f;
    public AudioSource source;
    public float lookSpeed = 3;

    [Header("Detection")]
    public float playerDetectionRadius;
    public float playerDetectionFOV;
    public float giveUpTime = 5;

    public bool canHearPlayer = false;
    public bool canSeePlayer = false;

    [Header("AI")]
    public Path enemyPath;
    public NavMeshAgent agent;
    private Transform playerRef;
    public EnemyState currentEnemyState = EnemyState.Patrolling;

    private Vector3 vel;
    private Vector3 lastPos;

    static string PATROL = "BeginPatrol";
    static string CHASE = "BeginChasing";

    PlayerController controller;

    private void Start ()
    {
        
        StartCoroutine("Footstep");
        StartCoroutine(PATROL);
        playerRef = GameObject.FindGameObjectWithTag("Player").transform;
        controller = playerRef.GetComponent<PlayerController>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update ()
    {
        

        Vector3 dif = playerRef.position - transform.position;

        float spread = -Vector3.Dot(Vector3.right, dif.normalized);

        source.panStereo = spread;
        source.volume = 1-(Mathf.Clamp(dif.magnitude-source.minDistance, 0, source.maxDistance)/(source.maxDistance-source.minDistance));
        
        if (dif.magnitude <= playerDetectionRadius && !controller.isWalking && controller.isMoving)
        {
            canHearPlayer = true;
            //Enemy can hear you

            if (currentEnemyState != EnemyState.Chasing)
            {
                currentEnemyState = EnemyState.Chasing;
                SwitchState();
            }
        }
        else canHearPlayer = false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dif, playerDetectionRadius, seeMask);

        if (dif.magnitude <= playerDetectionRadius && Vector3.Angle(transform.right,dif) <= playerDetectionFOV/2 && hit.transform == playerRef)
        {
            canSeePlayer = true;
            if (currentEnemyState != EnemyState.Chasing)
            {
                currentEnemyState = EnemyState.Chasing;
                SwitchState();
            }
        }
        else {
            canSeePlayer = false;
        }

        vel = transform.position - lastPos;
        lastPos = transform.position;

    }

    private void SwitchState ()
    {
        StopAllCoroutines();
        StartCoroutine("Footstep");

        if (currentEnemyState == EnemyState.Patrolling)
        {
            StartCoroutine(PATROL);
        }
        else if (currentEnemyState == EnemyState.Chasing)
        {
            StartCoroutine(CHASE);
        }
        else if (currentEnemyState == EnemyState.RandomSearching)
        {
            //Not implemented
            StartCoroutine(PATROL);
        }
    }

    private void LookAtPlayer ()
    {
        Vector3 mousePos = new Vector3(playerRef.position.x, playerRef.position.y, 0);
        
        mousePos.z = transform.position.z;


        if (Vector3.Distance(mousePos, transform.position) >= 0.2f)
        {
            LookAt(mousePos);
        }
    }

    private void LookAt (Vector3 pos)
    {
        Vector3 mousePos = pos - transform.position;
        mousePos.z = transform.position.z;
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.AngleAxis(angle, Vector3.forward),lookSpeed*Time.deltaTime);
    }


    IEnumerator BeginChasing ()
    {
        //Give time for player to stop walking near enemies
        yield return new WaitForSeconds(0.5f);
        float lostPlayerDuration = 0f;
        while (currentEnemyState == EnemyState.Chasing)
        {
            yield return new WaitForEndOfFrame();
            if (canSeePlayer || canHearPlayer)
            {
                lostPlayerDuration = 0f;
                agent.SetDestination(playerRef.position);
                LookAt(agent.steeringTarget);
            }
            else {
                agent.SetDestination(playerRef.position);
                LookAt(agent.steeringTarget);
                lostPlayerDuration += Time.deltaTime;
            }

            if (lostPlayerDuration >= giveUpTime)
            {
                Debug.Log("LOST PLAYER");
                currentEnemyState = EnemyState.Patrolling;
                SwitchState();
                break;
            }
        }
        
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
