using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;


public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private float _slowAcceleration = 1500f;
    [SerializeField]
    private float _slowAccelerationThresholdHigh = 35f;

    public PlayerState State;

    void Awake()
    {
        GameManager.Instance.PlayerController = this;
        _rb = GetComponentInChildren<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Cause an explosion
        }

        // Determine animation state changes between flying and running
        // Explosions cause the sheep to tumble
        // If it lands back on it's feet, it will run and jump forward
        // After landing, ease the rotations so the sheep will run forward
    }

    void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;

        // Add some extra thrust if player is moving too slow and is on feet
        if (_rb.velocity.z < _slowAccelerationThresholdHigh * dt)
        {
            _rb.AddForce(Vector3.forward * _slowAcceleration * dt, ForceMode.Acceleration);
        }
    }
}
