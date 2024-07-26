using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEmitter : MonoBehaviour
{
    [SerializeField] private GameObject ballPerfab;
    [SerializeField] private float spawnRate    = 0.1f;
    [SerializeField] private int maxParticles   = 3;
    [SerializeField] private Vector2 sizeRange;

    private GameObject[] pool;

    // Start is called before the first frame update
    void Start()
    {
        InitialPool();
        SpawnBall();
    }

    private void InitialPool()
	{
        pool = new GameObject[maxParticles];

        for(int i = 0; i < maxParticles; ++i)
		{
            pool[i] = Instantiate(ballPerfab);
            pool[i].SetActive(false);
        }       
    }

    private void SpawnBall()
    {
        foreach(GameObject ball in pool)
		{
            if(!ball.activeSelf)
			{
                ball.transform.position     = transform.TransformPoint(Random.insideUnitSphere * 0.5f);
                ball.transform.localScale   = Random.Range(sizeRange.x, sizeRange.y) * Vector3.one;
                ball.SetActive(true);
                break;
			}
		}
        
        Invoke("SpawnBall", spawnRate);
    }
}
