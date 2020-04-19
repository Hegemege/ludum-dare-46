using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class DropShadowController : MonoBehaviour
{
    public PlayerController PlayerController;

    public GameObject Marker;

    public GameObject BoostIndicator;

    private Vector3 _startPosition;

    void Awake()
    {
        _startPosition = transform.position;
        TrackPlayer();
    }

    void LateUpdate()
    {
        // Always position the dropshadow below the player
        TrackPlayer();

        BoostIndicator.SetActive(PlayerController.CanBoost);
    }

    private void TrackPlayer()
    {
        transform.position = new Vector3(PlayerController.transform.position.x, _startPosition.y, PlayerController.transform.position.z);
        // Set scale based on height
        var scaleT = Mathf.Clamp(PlayerController.transform.position.y / 5f, 0f, 1f);
        var scale = Mathf.Lerp(1.5f, 0.5f, scaleT);
        Marker.transform.localScale = Vector3.one * scale;
    }
}
