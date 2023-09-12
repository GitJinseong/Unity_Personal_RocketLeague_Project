using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTest : MonoBehaviour
{
    private RaycastHit rayTest;
    private Transform carTf;
    private Vector3 rayPosition;

    private float maxDistance = 20f;

    void Awake()
    {
        carTf = GetComponent<Transform>();
    }

    void Update()
    {
        rayPosition = carTf.transform.position + Vector3.up * 4;

        Debug.DrawRay(rayPosition, carTf.transform.right * maxDistance, Color.blue, 0.01f);
        if (Physics.Raycast(rayPosition, carTf.transform.right, out rayTest, maxDistance))
        {
            rayTest.transform.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }
}
