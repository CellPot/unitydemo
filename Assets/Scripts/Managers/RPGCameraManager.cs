using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RPGCameraManager : MonoBehaviour
{
    public static RPGCameraManager sharedInstance = null;
    [HideInInspector] 
    public CinemachineVirtualCamera virtualCamera;
    private void Awake()
    {
        //имплем. синглтона
        if (sharedInstance != null && sharedInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            sharedInstance = this;
        }
        GameObject vCamGameObject = GameObject.FindWithTag("VirtualCamera");
        if (vCamGameObject!=null)
        {
            virtualCamera = vCamGameObject.GetComponent<CinemachineVirtualCamera>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
