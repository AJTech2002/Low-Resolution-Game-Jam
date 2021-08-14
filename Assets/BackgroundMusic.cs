using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioClip chill;
    public AudioClip chasing;
    public AudioSource source;

    public Animation animationPlayer;

    public bool chasingBool = false;

    private void Start ()
    {
        source.clip = chill;
        source.Play();
    }

    public float delay = 2f;
    private float time = 0f;

    public void SwitchAudio (AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    public void PlayChill ()
    {
        if (chasingBool && time >= delay)
        {
            animationPlayer.Play("BlendChill");
            chasingBool = false;
            time = 0f;
        }
    }

    private void Update ()
    {
        time += Time.deltaTime;
    }

    public void PlayChase ()
    {
        if (!chasingBool && time >= delay)
        {
            animationPlayer.Play("BlendChase");
            chasingBool = true;
            time = 0f;
        }
    }
}
