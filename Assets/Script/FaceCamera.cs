using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(mainCamera == null){
            mainCamera = Camera.main;
        }
        if(mainCamera == null){
            return;
        }
        transform.rotation = mainCamera.transform.rotation;
    }
}
