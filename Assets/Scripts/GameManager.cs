using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Sound death;
    public Sound spawn;

    public Transform playerPrefab;

    

    [Header("Scenes (0 menu),[levels],(last end)")]
    public List<string> levelNames;

    private int currentScene;

    private void Awake()
    {
        if (GameObject.FindObjectsOfType<GameManager>().Length > 1) Destroy(this.gameObject);
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        instance = this;
        currentScene = levelNames.IndexOf(SceneManager.GetActiveScene().name);
            sceneSpawn = GameObject.FindGameObjectWithTag("Respawn").transform;
            if (sceneSpawn == null) Debug.LogError("Scene doesn't have a spawn point");
        
        if (this != null && GetComponent<AudioSource>() != null)
        spawn.PlayFromSource(GetComponent<AudioSource>(), 0, 3f);
        if (GameObject.FindObjectOfType<PlayerController>() == null)
        {
            
            Transform.Instantiate(playerPrefab, sceneSpawn.position, Quaternion.identity);
        }
    }

    private Transform sceneSpawn;

    private void Start()
    {
       
    }



    public static void ProgressScene ()
    {
        instance.Progress();
    }

    public static void PlayerDeath ()
    {
        instance.death.PlayAt(GameObject.FindObjectOfType<PlayerController>().transform.position, 1);
        instance.Death();
    }

    public void Progress ()
    {
        if (currentScene < levelNames.Count-1)
        {
            SceneManager.LoadScene(levelNames[currentScene + 1], LoadSceneMode.Single);
        }
    }

    public void Death ()
    {
        //Respawn at the start
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    

}
