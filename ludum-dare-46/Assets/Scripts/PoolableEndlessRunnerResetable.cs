using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class PoolableEndlessRunnerResetable : EndlessRunnerResetable, IResetableBehaviour
{
    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.KillEndlessObjects += ResetEndlessObjectToPool;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameManager.Instance.KillEndlessObjects -= ResetEndlessObjectToPool;
    }

    public void ResetEndlessObjectToPool(Vector3 point, Vector3 normal)
    {
        // Resets the object back to the pool if the position of object is beyond the
        // given kill plane defined as a point and normal
        var planeToObject = (transform.position - point).normalized;
        if (Vector3.Dot(planeToObject, normal) < 0)
        {
            Reset();
        }
    }

    public virtual void Initialize() { }
    public virtual void Reset()
    {
        gameObject.SetActive(false);
    }
}
