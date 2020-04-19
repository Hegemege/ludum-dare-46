using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class StreetController : MonoBehaviour
{
    private List<float> _sideScales = new List<float> { -15f, -22.5f, 15f, 22.5f, 30f };

    public int SpawnLimiter = 0;

    void Awake()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("CameraSpawner")) return;
        if (SpawnLimiter > 0)
        {
            SpawnLimiter -= 1;
            return;
        }

        // CameraSpawner has his the street spawn trigger
        // Spawn a random environment piece
        foreach (var side in _sideScales)
        {
            if (Random.Range(0f, 1f) > 0.25f)
            {
                var environment = PoolManager.Instance.GetRandomEnvironmentSpawnable();

                environment.transform.position = transform.position + Vector3.right * -1f * side + Vector3.forward * Random.Range(-3f, 3f);
            }
        }


        // Spawn a random obstacle
        if (Random.Range(0f, 1f) > 0.75f)
        {
            var obstacle = PoolManager.Instance.GetRandomObstacle();
            var offsetX = Random.Range(-4.5f, 4.5f);
            var offsetZ = Random.Range(-4.5f, 4.5f);
            var offset = new Vector3(offsetX, 0f, offsetZ);

            obstacle.transform.position = transform.position + offset;
        }


    }
}
