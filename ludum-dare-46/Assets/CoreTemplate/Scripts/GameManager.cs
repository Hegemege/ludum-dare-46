﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class GameManager : GenericManager<GameManager>, ILoadedManager
{
    public DataStore DataStore;

    public delegate void ResetEndlessObjectsEvent(Vector3 offset);
    public delegate void KillEndlessObjectsEvent(Vector3 point, Vector3 normal);
    public ResetEndlessObjectsEvent ResetEndlessObjects;
    public KillEndlessObjectsEvent KillEndlessObjects;

    // Repeating environment cycle in units of space
    public float ResetStepZ = 10f;
    public float ResetThreshold = 500f;

    public PlayerController PlayerController;
    public CameraController CameraController;

    public void Initialize()
    {
        if (!InitializeSingleton(this)) return;
    }

    public void PostInitialize() { }

    public void TriggerReset()
    {
        // Figure out how many environment cycles all objects are moved back
        var distance = PlayerController.GetDistance();
        var cycles = Mathf.FloorToInt(distance.z / ResetStepZ);

        var moveBackDistance = cycles * ResetStepZ;
        var moveOffset = Vector3.forward * -1f * moveBackDistance;

        // Move the player and all endless objects
        PlayerController.transform.position += moveOffset;
        ResetEndlessObjects(moveOffset);

        // Reset objects that are too far back
        KillEndlessObjects(CameraController.BehindKillPlane.position, Vector3.forward);
        // Reset objects that have fallen below the level
        KillEndlessObjects(new Vector3(0f, -5f, 0f), Vector3.up);
    }
}
