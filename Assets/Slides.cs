using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slides : MonoBehaviour
{
    public List<GameObject> slides = new List<GameObject>();
    public int currentSlide;

    private float minDuration = 0.2f;
    private float dur = 0f;

    public UnityEngine.Canvas canvas;
    public Camera cam;
    private void Awake ()
    {
        canvas.worldCamera = cam;
        for (int i = 0; i < slides.Count; i++)
        {
            if (i == currentSlide) { slides[i].SetActive(true); continue; }
            slides[i].SetActive(false);
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && dur >= minDuration)
        {
            slides[currentSlide].SetActive(false);
            currentSlide++;

            if (currentSlide > slides.Count-1) {
                GameManager.ProgressScene();
            }
            else
            {
                slides[currentSlide].SetActive(true);
            }
            dur = 0f;
        }

        dur += Time.deltaTime;
    }
}
