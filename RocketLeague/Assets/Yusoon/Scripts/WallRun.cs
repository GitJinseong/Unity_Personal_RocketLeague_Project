using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    public LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 레이 시작 위치
        Vector3 rayStart = transform.position;
        // 레이의 방향 (오른쪽으로)
        Vector3 rayDirection = Vector3.forward;
        // 레이의 최대 거리
        float rayDistance = 3f;

        // 레이캐스트를 사용하여 충돌 검사
        RaycastHit hitWall;
        if (Physics.Raycast(rayStart, rayDirection, out hitWall, rayDistance, layerMask))
        {
            // 충돌한 오브젝트의 이름을 가져와서 출력
            string name = hitWall.collider.name;
            Debug.Log("Hit object name: " + name);

            // 레이 시각적으로 표시
            Debug.DrawRay(rayStart, rayDirection * rayDistance, Color.red);
        }
        else
        {
            // 레이 시각적으로 표시 (레이가 충돌하지 않은 경우)
            Debug.DrawRay(rayStart, rayDirection * rayDistance, Color.green);
        }

    }
}
