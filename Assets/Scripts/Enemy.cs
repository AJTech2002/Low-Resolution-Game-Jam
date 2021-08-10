using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    [Header("SFX")]
    public List<AudioClip> footSteps;
    public float minFootstepDelay = 0.8f;
    public float maxFootstepDelay = 1.2f;
    public AudioSource source;

    [Header("AI")]
    public NavMeshAgent agent;
    private Transform playerRef;
    private void Start ()
    {
        StartCoroutine("Footstep");
        playerRef = GameObject.FindGameObjectWithTag("Player").transform;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update ()
    {
        agent.SetDestination(playerRef.position);

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
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
