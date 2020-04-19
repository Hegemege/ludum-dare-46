using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class TrapController : PoolableEndlessRunnerResetable
{
    [SerializeField]
    private Animator _animator;

    private bool _sprung;

    public void Spring()
    {
        _sprung = true;
        _animator.SetTrigger("Trigger");
    }

    public override void Reset()
    {
        if (_sprung)
        {
            _animator.SetTrigger("Reset");
        }

        _sprung = false;

        base.Reset();
    }
}
