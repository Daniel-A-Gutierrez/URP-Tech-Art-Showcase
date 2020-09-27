using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBalls : MonoBehaviour
{
    public int N;
    public GameObject ballPrefab;
    public Vector3 spawnPosition;
    public float spawnRadius;
    void Start()
    {
        for(int i = 0; i < N; i++)
        {
            Random.InitState( i*43 );
            GameObject g = Instantiate(ballPrefab,
                spawnPosition + new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius)),
                Quaternion.identity);
            g.GetComponent<Colorer>().ChangeColor(new Color(Random.Range(.2f, .9f), Random.Range(.2f, .9f), Random.Range(.2f, .9f)) , 1.5f);
            g.GetComponent<Rigidbody>().mass = Random.Range(1.0f, 10.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
