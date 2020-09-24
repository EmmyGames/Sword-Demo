using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    public Renderer Renderer;
    private Color _startColor = Color.white;
    private Color _damageColor = Color.red;
    private ObjectStats _objectStats;
    private void Start()
    {
        _objectStats = GetComponent<ObjectStats>();
        Renderer.material.color = _startColor;
    }

    private void Update()
    {
        if (_objectStats.health <= 0)
        {
            ChangeColor();
            Destroy(this.gameObject, 1);
        }
        
    }

    private void ChangeColor()
    {
        Renderer.material.color = _damageColor;
    }
}
