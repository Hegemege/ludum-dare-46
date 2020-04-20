using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;
using TMPro;
using UnityEngine.SceneManagement;

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

    public bool CanBoost;

    public GameObject EndCanvas;
    public GameObject CreditCanvas;
    public TextMeshProUGUI DistanceText;

    private Vector3 _endCanvasStartOffset;
    private Vector3 _creditCanvasStartOffset;


    void Awake()
    {
        GameManager.Instance.DistanceTracker = 0f;
        GameManager.Instance.PlayerController = this;
        _rb = GetComponentInChildren<Rigidbody>();
        _flyingParticles = GetComponentInChildren<ParticleSystem>();
        _emissionModule = _flyingParticles.emission;
        _flyingParticlesStartEmissionRate = _emissionModule.rateOverTime.constant;
        _startPosition = transform.position;
        State = PlayerState.Running;

        _endCanvasStartOffset = EndCanvas.transform.localPosition;
        _creditCanvasStartOffset = CreditCanvas.transform.localPosition;
    }

    void Update()
    {
        // Determine animation state changes between flying and running
        // Explosions cause the sheep to tumble
        // If it lands back on it's feet, it will run and jump forward
        // After landing, ease the rotations so the sheep will run forward
        _animator.SetBool("Running", State == PlayerState.Running);
        _animator.SetBool("Flying", State == PlayerState.Flying);
        _animator.SetBool("Idle", State == PlayerState.Dead);

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

        if (State == PlayerState.Dead)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.ResetLevel();
                SceneManager.LoadScene("main");
                return;
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && CanBoost)
        {
            BoostExplode();
        }
    }

    public void BoostExplode()
    {
        CanBoost = false;

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
        if (State == PlayerState.Dead) return;

        PoolManager.Instance.ExplosionAudioPool.GetPooledObject();

        if (State == PlayerState.Running)
        {
            _startFlyingTimer = 0f;
        }
        State = PlayerState.Flying;
        _rb.constraints = RigidbodyConstraints.None;
    }

    void FixedUpdate()
    {
        if (State == PlayerState.Dead) return;

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
        if (State == PlayerState.Dead) return;

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

                // Dust
                if (Mathf.Abs(other.relativeVelocity.y) > 3f)
                {
                    var ps = PoolManager.Instance.DustParticlePool.GetPooledObject();
                    ps.gameObject.transform.position = transform.position + Vector3.forward * 0.5f;
                }

                // Boing
                if (Mathf.Abs(other.relativeVelocity.y) > 4f)
                {
                    PoolManager.Instance.JumpAudioPool.GetPooledObject();
                }
            }

        }
    }

    void OnTriggerEnter(Collider other)
    {
        var dead = false;
        // If player manages to escape
        if (other.CompareTag("PlayerDeathTrigger"))
        {
            transform.position = new Vector3(0f, 1f, transform.position.z);
            dead = true;
        }

        if (other.CompareTag("ExplosionTrigger"))
        {
            ObstacleExplode();

            // Give an extra boost for each hit explosion boost
            CanBoost = true;

            var trigger = other.GetComponent<ExplosionTrigger>();
            if (trigger != null && trigger.Root != null)
            {
                trigger.Root.SetActive(false);
            }
        }

        if (other.CompareTag("Trap") || dead)
        {
            var trap = other.GetComponentInParent<TrapController>();
            if (trap != null)
            {
                PoolManager.Instance.TrapHitAudioPool.GetPooledObject();
                trap.Spring();
            }

            // Stop player
            State = PlayerState.Dead;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
            CanBoost = false;

            EndCanvas.SetActive(true);
            EndCanvas.transform.rotation = Quaternion.identity;
            EndCanvas.transform.position = transform.position + _endCanvasStartOffset;
            EndCanvas.transform.position = new Vector3(_endCanvasStartOffset.x, _endCanvasStartOffset.y, EndCanvas.transform.position.z);

            CreditCanvas.SetActive(true);
            CreditCanvas.transform.rotation = Quaternion.identity;
            CreditCanvas.transform.position = transform.position + _creditCanvasStartOffset;
            CreditCanvas.transform.position = new Vector3(_creditCanvasStartOffset.x, _creditCanvasStartOffset.y, CreditCanvas.transform.position.z);

            PoolManager.Instance.LoseAudioPool.GetPooledObject();

            var totalDistance = GameManager.Instance.DistanceTracker + (transform.position - _startPosition).z;
            DistanceText.text = Mathf.FloorToInt(totalDistance).ToString() + "m";

            // Move the player upside down into the spikes
            //transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 180f);
            //transform.position = other.gameObject.transform.position + Vector3.up;

            // Spawn death particles
            var ps = PoolManager.Instance.DeathParticlePool.GetPooledObject();
            ps.gameObject.transform.position = transform.position;
        }
    }
}
