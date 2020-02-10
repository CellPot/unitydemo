using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGGameManager : MonoBehaviour
{
    public static RPGGameManager sharedInstance = null;
    public SpawnPoint playerSpawnPoint;
    public RPGCameraManager cameraManager;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        SetupScene();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (playerSpawnPoint.objectsAtScene == 0)
        {
            SetupScene();
        }
    }

    private void Awake()
    {
        if (sharedInstance !=null && sharedInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            sharedInstance = this;
        }
    }
    public void SetupScene()
    {
        SpawnPlayer();
    }
    public void SpawnPlayer()
    {
        if (playerSpawnPoint!= null)
        {
            player = playerSpawnPoint.SpawnObject();
            cameraManager.virtualCamera.Follow = player.transform;            
        }
    }
}
