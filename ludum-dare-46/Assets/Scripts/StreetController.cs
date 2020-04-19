using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class StreetController : MonoBehaviour
{
    void Awake()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("CameraSpawner")) return;

        // CameraSpawner has his the street spawn trigger
        // Spawn a random environment piece close to the street
        // on a random side
        if (Random.Range(0f, 1f) > 0.25f)
        {
            var environment = PoolManager.Instance.GetRandomEnvironmentSpawnable();
            var side = Random.Range(0f, 1f) > 0.5f ? 1f : -1f;

            environment.transform.position = transform.position + Vector3.right * side * 7.5f + Vector3.forward * Random.Range(-3f, 3f);
        }

        // Spawn a random environment piece further from the street
        if (Random.Range(0f, 1f) > 0.05f)
        {
            var environment = PoolManager.Instance.GetRandomEnvironmentSpawnable();
            var side = Random.Range(0f, 1f) > 0.5f ? 1f : -1f;

            environment.transform.position = transform.position + Vector3.right * side * 15f + Vector3.forward * Random.Range(-3f, 3f);
        }
    }
}
