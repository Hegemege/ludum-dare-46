using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;

    void Awake()
    {
        GameManager.Instance.PlayerController = this;
        _rb = GetComponentInChildren<Rigidbody>();
    }

    void Update()
    {

    }
}
