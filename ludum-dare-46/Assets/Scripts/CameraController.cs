using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class CameraController : EndlessRunnerResetable
{
    // Keep the camera positioned fixed distance from the center of the track
    // But keep it following the player

    // Thus we only need to move it forward (positive Z)

    private Vector3 _startPosition;

    private Vector3 _targetEasedPosition;
    [SerializeField]
    private float _easing = 0.05f;

    [SerializeField]
    private BoxCollider Spawner;

    public GameObject BehindKillPlane;

    private Camera _camera;

    private float _targetFarPlane;
    [SerializeField]
    private float _introAnimationLength;

    void Awake()
    {
        GameManager.Instance.CameraController = this;
        _camera = GetComponent<Camera>();
        _targetFarPlane = _camera.farClipPlane;
        StartCoroutine(AnimateFarClipPlane());
        _startPosition = transform.position;

        // Move the spawner at the start next to the camera
        Spawner.transform.position = new Vector3(0f, 0f, transform.position.z);
        Spawner.transform.rotation = Quaternion.identity;

        // Keep the kill plane behind the camera at 10 units
        BehindKillPlane.transform.position = transform.position + Vector3.back * 10f;
    }

    private IEnumerator AnimateFarClipPlane()
    {
        var t = 0f;
        while (t < _introAnimationLength)
        {
            var animationT = Mathf.Clamp01(t / _introAnimationLength);
            _camera.farClipPlane = Mathf.Lerp(1f, _targetFarPlane, animationT);

            t += Time.deltaTime;
            yield return null;
        }
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        // Follow player
        FollowPlayer();

        transform.position = Vector3.Lerp(transform.position, _targetEasedPosition, _easing);

        // Move the spawner back to x = 0, y = 0 always
        // The spawner's position is lerped to the camera's far clip plane in order to spawn objects
        var farClipPlane = _camera.farClipPlane;
        var spawnerTargetPosition = new Vector3(0f, 0f, transform.position.z + farClipPlane);
        Spawner.transform.position = Vector3.Lerp(Spawner.transform.position, spawnerTargetPosition, 0.1f);
        Spawner.transform.rotation = Quaternion.identity;
    }

    private void FollowPlayer()
    {
        var targetDistance = GameManager.Instance.PlayerController.GetDistance();
        // Track Z distance and height
        var zDistance = targetDistance.z;
        var yDistance = targetDistance.y;

        _targetEasedPosition = _startPosition + Vector3.forward * zDistance + Vector3.up * yDistance;
    }
}
