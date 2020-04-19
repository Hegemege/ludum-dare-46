using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class ManagerLoader : MonoBehaviour
{
    void Awake()
    {
        // Temporary container for all managers to be released. Setting a child's parent as null will skip an entry
        // if we iterate while doing it
        // Also, GetComponentInChildren<ILoadedManager>() will not help since we cannot access the transform anymore
        var children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        var managers = new List<ILoadedManager>();

        // Initialize and release are managers so they can be set as DontDestroyOnLoad
        foreach (var child in children)
        {
            var manager = child.GetComponent<ILoadedManager>();
            managers.Add(manager);

            if (manager != null)
            {
                child.parent = null;
                manager.Initialize();
            }
        }

        // Execute post-initialize
        foreach (var manager in managers)
        {
            manager.PostInitialize();
        }

        // Finally remove self
        Destroy(gameObject);
    }
}