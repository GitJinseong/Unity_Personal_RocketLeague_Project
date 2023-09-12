using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook1 : MonoBehaviour
{
    public Transform kartTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       transform.LookAt(kartTransform.right);
        
    }
}
