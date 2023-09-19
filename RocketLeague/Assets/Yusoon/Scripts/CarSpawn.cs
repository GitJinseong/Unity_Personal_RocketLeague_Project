using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawn : MonoBehaviour
{
    public GameObject[] orangeCar;
    public GameObject[] BlueCar;
    public GameObject ball;
    public Transform ballSpawnPosition;
    public Transform[] blueSpawnPosition=new Transform[3];
    public Transform[] orangeSpawnPosition=new Transform[3];

    int[] orangeRandomIdx=new int[3];
    int[] blueRnadomIdx = new int[3];

    void Start()
    {

        for(int i=0;i<orangeRandomIdx.Length; i++)
        {
            orangeRandomIdx[i]=i;
            blueRnadomIdx[i]=i;
        }

        for(int i=0;i<100;i++)
        {
            int randomIdx1 = Random.Range(0, 3);
            int randomIdx2 = Random.Range(0, 3);

            ShuffleIdx(orangeRandomIdx[randomIdx1], orangeRandomIdx[randomIdx2]);
            ShuffleIdx(blueRnadomIdx[randomIdx1], blueRnadomIdx[randomIdx2]);

        }

        for(int i=0;i<orangeSpawnPosition.Length;i++)
        {
            orangeCar[i].transform.position= orangeSpawnPosition[orangeRandomIdx[i]].position;
            orangeCar[i].transform.rotation=orangeSpawnPosition[orangeRandomIdx[i]].rotation;

        }
        for(int i=0;i<blueSpawnPosition.Length;i++)
        {
            BlueCar[i].transform.position=blueSpawnPosition[blueRnadomIdx[i]].position;
            BlueCar[i].transform.rotation=blueSpawnPosition[blueRnadomIdx[i]].rotation;

        }
        ball.transform.position=ballSpawnPosition.position;
    }

    private void ShuffleIdx(int a, int b)
    {
        int temp = a;
        a=b;
        b=temp;
    }
}
