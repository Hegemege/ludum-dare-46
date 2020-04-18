using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public abstract class EndlessRunnerResetable : MonoBehaviour, IResetableBehaviour
{
    public abstract void Initialize();
    public abstract void Reset();

    public void ResetEndlessPosition(Vector3 offset)
    {
        // Resets the position of the object using the given offset
        transform.position += offset;
    }
}
