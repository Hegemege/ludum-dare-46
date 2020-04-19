using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class TrapController : PoolableEndlessRunnerResetable
{
    [SerializeField]
    private Animator _animator;

    private bool _sprung;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (_sprung)
        {
            _animator.SetTrigger("Reset");
            _sprung = false;
        }
    }

    public void Spring()
    {
        if (_sprung) return;

        _sprung = true;
        _animator.SetTrigger("Trigger");
    }

    public override void Reset()
    {
        base.Reset();
    }
}
