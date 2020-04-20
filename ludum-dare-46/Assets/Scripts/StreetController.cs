using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class StreetController : MonoBehaviour
{
    private List<float> _sideScales = new List<float> { -15f, -22.5f, 15f, 22.5f, 30f };

    public int SpawnLimiter = 0;

    private List<Vector3> _spawnPositions = new List<Vector3> {
        new Vector3(-2.5f, 0f, -2.5f),
        new Vector3(2.5f, 0f, -2.5f),
        new Vector3(0f, 0f, 0f),
        new Vector3(-2.5f, 0f, 2.5f),
        new Vector3(2.5f, 0f, 2.5f)
    };

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
        foreach (var position in _spawnPositions)
        {
            if (Random.Range(0f, 1f) > 0.8f)
            {
                var obstacle = PoolManager.Instance.GetRandomObstacle();
                var offsetX = Random.Range(-1f, 1f);
                var offsetZ = Random.Range(-1f, 1f);
                var offset = new Vector3(offsetX, 0f, offsetZ);

                obstacle.transform.position = transform.position + position + offset;
                obstacle.transform.rotation = Quaternion.identity;
            }

            // Spawn a trap high in the air
            if (Random.Range(0f, 1f) > 0.92f)
            {
                var obstacle = PoolManager.Instance.AirTrapPool.GetPooledObject();
                var offsetX = Random.Range(-1f, 1f);
                var offsetZ = Random.Range(-1f, 1f);
                var offsetY = Random.Range(25f, 50f);
                var offset = new Vector3(offsetX, offsetY, offsetZ);

                obstacle.gameObject.transform.position = transform.position + position + offset;
                obstacle.gameObject.transform.rotation = Random.rotation;
            }
        }


    }
}
