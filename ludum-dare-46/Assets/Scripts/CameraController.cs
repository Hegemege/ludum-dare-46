using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class CameraController : MonoBehaviour
{
    // Keep the camera positioned fixed distance from the center of the track
    // But keep it following the player

    // Thus we only need to move it forward (positive Z)

    private Vector3 _startPosition;
    private Vector3 _targetStartPosition;

    private Transform _target;

    private Vector3 _targetEasedPosition;
    [SerializeField]
    private float _easing = 0.05f;

    void Awake()
    {
        _target = GameManager.Instance.PlayerController.transform;
        _startPosition = transform.position;
        _targetStartPosition = _target.position;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        // Follow player
        FollowPlayer();

        // TODO: Easing
        transform.position = Vector3.Lerp(transform.position, _targetEasedPosition, _easing);
    }

    private void FollowPlayer()
    {
        // Get the Z distance of the player and track that
        var zDistance = (_target.position - _targetStartPosition).z;
        // Track height too
        var yDistance = (_target.position - _targetStartPosition).y;

        _targetEasedPosition = _startPosition + Vector3.forward * zDistance + Vector3.up * yDistance;
    }
}
