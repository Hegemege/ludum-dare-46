using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class GameManager : GenericManager<GameManager>, ILoadedManager
{
    public DataStore DataStore;

    // Store all resetable objects for the endless runner
    public List<EndlessRunnerResetable> ResetableObjects;

    // Repeating environment cycle in units of space
    public float ResetStepZ;
    public float ResetThreshold = 500;

    public PlayerController PlayerController;

    public void Initialize()
    {
        if (!InitializeSingleton(this)) return;
    }

    public void PostInitialize() { }

    public void ResetEndlessObjects()
    {
        // Figure out how many environment cycles all objects are moved back

    }
}
