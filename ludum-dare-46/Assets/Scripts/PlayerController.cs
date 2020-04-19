﻿using System.Collections;
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

    private Vector3 _startPosition;

    [SerializeField]
    private float BoostForceMin;
    [SerializeField]
    private float BoostForceMax;

    [SerializeField]
    private LayerMask StreetLayerMask;

    private float _startFlyingTimer;

    [SerializeField]
    private float _steeringForce;

    void Awake()
    {
        GameManager.Instance.PlayerController = this;
        _rb = GetComponentInChildren<Rigidbody>();
        _startPosition = transform.position;
        State = PlayerState.Running;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Cause an explosion
            var ps = PoolManager.Instance.ExplosionParticlePool.GetPooledObject();
            ps.gameObject.transform.position = transform.position + Vector3.up * 0.5f;

            var randomForwardForce = Vector3.forward * 2f + Vector3.up * 4f + Random.onUnitSphere;
            randomForwardForce.Normalize();
            ExplosionHit();
            _rb.AddForce(randomForwardForce * Random.Range(BoostForceMin, BoostForceMax), ForceMode.Impulse);
            _rb.AddTorque(Random.onUnitSphere * 0.1f, ForceMode.Impulse);
        }

        // Determine animation state changes between flying and running
        // Explosions cause the sheep to tumble
        // If it lands back on it's feet, it will run and jump forward
        // After landing, ease the rotations so the sheep will run forward
        _animator.SetBool("Running", State == PlayerState.Running);
        _animator.SetBool("Flying", State == PlayerState.Flying);
    }

    public void ExplosionHit()
    {
        if (State == PlayerState.Running)
        {
            _startFlyingTimer = 0f;
        }
        State = PlayerState.Flying;
        _rb.constraints = RigidbodyConstraints.None;
    }

    void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;

        // Add some extra thrust if player is moving too slow and is on feet
        if (_rb.velocity.z < _slowAccelerationThresholdHigh * dt)
        {
            _rb.AddForce(Vector3.forward * _slowAcceleration * dt, ForceMode.Acceleration);
        }

        // Trigger reset via GameManager if we go too far forward
        if (GetDistance().z > GameManager.Instance.ResetThreshold)
        {
            GameManager.Instance.TriggerReset();
        }

        // Horizontal steering
        var horizontalInput = Input.GetAxis("Horizontal");
        // Running allows better steering
        var steeringScale = State == PlayerState.Running ? 2f : 1f;
        _rb.AddForce(Vector3.right * horizontalInput * _steeringForce * steeringScale * dt, ForceMode.Acceleration);

        // Time since flying started - flying can't end too early
        _startFlyingTimer += dt;

        // If the player is running, lock rotations and smooth-move the player back up-right
        if (State == PlayerState.Running)
        {
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, 0.05f);
        }
        else
        {
            _rb.constraints = RigidbodyConstraints.None;
        }

        // Raycast down - if we hit floor, go into running state
        if ((State == PlayerState.Flying || State == PlayerState.Tumbling) && _startFlyingTimer > 1.2f)
        {
            if (Physics.Raycast(transform.position, transform.up * -1f, 1f, StreetLayerMask))
            {
                State = PlayerState.Running;
            }
        }
    }

    public Vector3 GetDistance()
    {
        return transform.position - _startPosition;
    }

    void OnCollisionEnter(Collision other)
    {
        var ownCollider = other.contacts[0].thisCollider;
        // If the upper collider hits something, start "tumbling"
        if (ownCollider.CompareTag("PlayerUpperCollider"))
        {
            State = PlayerState.Tumbling;
        }
        else
        {
            // End flying if we land feet first on a street
            if (other.gameObject.CompareTag("Street"))
            {
                State = PlayerState.Running;
            }

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerDeathTrigger"))
        {
            print("DEATH");
        }
    }
}
