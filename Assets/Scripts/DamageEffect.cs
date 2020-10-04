using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    private PlayerStats _stats;
    private void Start()
    {
        _stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (_stats.health <= 0)
        {
            Destroy(this.gameObject, 1);
        }
    }
}
