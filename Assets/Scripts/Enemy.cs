

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

    [Header("Speeds")]
    public float lookSpeed = 3;
    public float hearAgentSpeed;
    public float seeAgentSpeed;

    [Header("Detection")]
    public float playerDetectionRadius;
    public float playerDetectionFOV;
    public float giveUpTime = 5;

    public MeshRenderer rendererObj;
    public Color okay;
    public Color seen;
    public Color hear;

    [Header("Debug")]
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

        dif.z = 0f;

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

        if (Vector3.Distance(playerRef.position,transform.position) <= 0.9f && Vector3.Dot(transform.right, dif) >= 0.5f)
        {
            GameManager.PlayerDeath();
        }

        if (canSeePlayer && canHearPlayer || canSeePlayer)
        {
            rendererObj.materials[0].SetColor("_Color", seen);
        }
        else if (canHearPlayer && !canSeePlayer)
        {
            rendererObj.materials[0].SetColor("_Color", hear);
        }
        else if (!canHearPlayer && !canSeePlayer)
        {
            rendererObj.materials[0].SetColor("_Color", okay);
        }
        else {
             rendererObj.materials[0].SetColor("_Color", okay);
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

        float lostPlayerDuration = 0f;
        Vector3 lastKnownPosition = new Vector3();
        while (currentEnemyState == EnemyState.Chasing)
        {
            Vector3 dif = playerRef.position - transform.position;
            if (dif.magnitude <= playerDetectionRadius && !controller.isWalking && controller.isMoving)
            {
                canHearPlayer = true;

            }
            else canHearPlayer = false;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dif, playerDetectionRadius, seeMask);

            Debug.DrawLine(transform.position, playerRef.position, Color.red,0.1f);

            Debug.Log(dif.magnitude + " but " +  (hit.transform == playerRef));
            if (dif.magnitude <= playerDetectionRadius && Vector3.Angle(transform.right,dif) <= playerDetectionFOV/2 && hit.transform == playerRef)
            {
                canSeePlayer = true;

            }
            else {
                
                canSeePlayer = false;
            }


            yield return new WaitForEndOfFrame();
            if (canSeePlayer || canHearPlayer)
            {

                if (canHearPlayer) agent.speed = hearAgentSpeed;
                if (canSeePlayer) agent.speed = seeAgentSpeed;

                lastKnownPosition = playerRef.position;

                lostPlayerDuration = 0f;
                if (agent.isActiveAndEnabled)
                agent.SetDestination(playerRef.position);
                LookAt(agent.steeringTarget);
            }
            else {
                if (lostPlayerDuration <= 0.5f) lastKnownPosition = playerRef.position;
                if (agent.isActiveAndEnabled)
                agent.SetDestination(lastKnownPosition);
                LookAt(agent.steeringTarget);

                if (Vector3.Distance(transform.position,lastKnownPosition) <= 1f)
                    lostPlayerDuration = giveUpTime;

                lostPlayerDuration += Time.deltaTime;
            }

            if (lostPlayerDuration >= giveUpTime && !(canSeePlayer || canHearPlayer))
            {

                currentEnemyState = EnemyState.Patrolling;
                SwitchState();
                break;
            }
        }
        
        currentEnemyState = EnemyState.Patrolling;
                SwitchState();
        
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
                agent.speed = hearAgentSpeed;

                if (agent.path.corners.Length > 0)
                    newSteeringTarget = Vector3.Lerp(newSteeringTarget,agent.steeringTarget,5*Time.deltaTime);
                else
                    newSteeringTarget = Vector3.Lerp(newSteeringTarget,enemyPath.path[pathIndex],5*Time.deltaTime);

                LookAt(newSteeringTarget);
                
                if (agent.isActiveAndEnabled)
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

 //Stores an AudioClip and some functions to use them
[System.Serializable]
public class Sound {
    public AudioClip sound;
    [Range(0f, 2f)]
    public float defaultVolume;

    IEnumerator RunIn (float delay, Vector3 position, float volume)
    {
        yield return new WaitForSeconds(delay);
        PlayAt(position, volume, 0f);
    }

    IEnumerator RunSourceIn (float delay, AudioSource source, float volume)
    {
        yield return new WaitForSeconds(delay);
        PlayFromSource(source, 0f, volume);
    }

    //Play from a position
    public void PlayAt (Vector3 position, float volume = -1f, float delay = 0f)
    {
        if (volume < 0) volume = defaultVolume;
        if (sound != null)
        {
            if (delay == 0f)
                AudioSource.PlayClipAtPoint(sound, position, volume);
            else
                 GameObject.FindObjectOfType<PlayerController>().StartCoroutine(RunIn(delay, position, volume));
        }

    }
    
    //Play from a source this clip
    public void PlayFromSource (AudioSource source, float delay=0f, float volume = -1f)
    {
        if (volume < 0) volume = defaultVolume;
        if (sound != null)
        {
            if (delay == 0f)
                source.PlayOneShot(sound, volume);
            else
                GameObject.FindObjectOfType<PlayerController>().StartCoroutine(RunSourceIn(delay, source, volume));
        }

    }

}