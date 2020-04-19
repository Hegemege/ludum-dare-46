using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class EndlessRunnerResetable : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        GameManager.Instance.ResetEndlessObjects += ResetEndlessPosition;
    }

    protected virtual void OnDisable()
    {
        GameManager.Instance.ResetEndlessObjects -= ResetEndlessPosition;
    }

    public void ResetEndlessPosition(Vector3 offset)
    {
        // Resets the position of the object using the given offset
        transform.position += offset;
    }
}
