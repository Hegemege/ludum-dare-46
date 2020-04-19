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

    private Vector3 _startPosition;

    [SerializeField]
    private float BoostForceMin;
    [SerializeField]
    private float BoostForceMax;

    [SerializeField]
    private float ExplosionForceMin;
    [SerializeField]
    private float ExplosionForceMax;


    [SerializeField]
    private LayerMask StreetLayerMask;

    private float _startFlyingTimer;

    [SerializeField]
    private float _steeringForce;

    private ParticleSystem _flyingParticles;
    private ParticleSystem.EmissionModule _emissionModule;
    private float _flyingParticlesStartEmissionRate;


    void Awake()
    {
        GameManager.Instance.PlayerController = this;
        _rb = GetComponentInChildren<Rigidbody>();
        _flyingParticles = GetComponentInChildren<ParticleSystem>();
        _emissionModule = _flyingParticles.emission;
        _flyingParticlesStartEmissionRate = _emissionModule.rateOverTime.constant;
        _startPosition = transform.position;
        State = PlayerState.Running;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BoostExplode();
        }

        // Determine animation state changes between flying and running
        // Explosions cause the sheep to tumble
        // If it lands back on it's feet, it will run and jump forward
        // After landing, ease the rotations so the sheep will run forward
        _animator.SetBool("Running", State == PlayerState.Running);
        _animator.SetBool("Flying", State == PlayerState.Flying);

        if (State == PlayerState.Flying)
        {
            _emissionModule.rateOverTime = _flyingParticlesStartEmissionRate;
        }
        else if (State == PlayerState.Tumbling)
        {
            _emissionModule.rateOverTime = _flyingParticlesStartEmissionRate / 2f;
        }
        else
        {
            _emissionModule.rateOverTime = 0f;
        }
    }

    public void BoostExplode()
    {
        var ps = PoolManager.Instance.ExplosionParticlePool.GetPooledObject();
        ps.gameObject.transform.position = transform.position + Vector3.up * 0.5f;

        var randomForwardForce = Vector3.forward * 6f + Vector3.up * 5f + Random.onUnitSphere;
        randomForwardForce.Normalize();
        ExplosionHit();
        _rb.AddForce(randomForwardForce * Random.Range(BoostForceMin, BoostForceMax), ForceMode.Impulse);
        _rb.AddTorque(Random.onUnitSphere * 0.1f, ForceMode.Impulse);
    }

    public void ObstacleExplode()
    {
        var ps = PoolManager.Instance.ExplosionParticlePool.GetPooledObject();
        ps.gameObject.transform.position = transform.position + Vector3.up * 0.5f;

        var randomForwardForce = Vector3.forward * 4f + Vector3.up * 5f + Random.onUnitSphere;
        randomForwardForce.Normalize();
        ExplosionHit();
        _rb.AddForce(randomForwardForce * Random.Range(ExplosionForceMin, ExplosionForceMax), ForceMode.Impulse);
        _rb.AddTorque(Random.onUnitSphere * 0.15f, ForceMode.Impulse);
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
                // Also give a small speed boost when landing
                if (State == PlayerState.Flying)
                {
                    _rb.AddForce(Vector3.forward * 10f, ForceMode.Impulse);
                }

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

        if (other.CompareTag("ExplosionTrigger"))
        {
            ObstacleExplode();

            var trigger = other.GetComponent<ExplosionTrigger>();
            if (trigger != null && trigger.Root != null)
            {
                trigger.Root.SetActive(false);
            }
        }
    }
}
