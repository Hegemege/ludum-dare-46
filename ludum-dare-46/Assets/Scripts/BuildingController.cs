using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class BuildingController : PoolableEndlessRunnerResetable
{
    [SerializeField]
    private List<MeshRenderer> _windows;

    [SerializeField]
    private Color _litColor;
    [SerializeField]
    private Color _unlitColor;

    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (var window in _windows)
        {
            window.enabled = Random.Range(0f, 1f) > 0.05f;
            var color = Random.Range(0f, 1f) > 0.65f ? _litColor : _unlitColor;
            if (Random.Range(0f, 1f) > 0.995f)
            {
                color = Color.red;
            }
            window.material.color = color;
        }
    }
}
