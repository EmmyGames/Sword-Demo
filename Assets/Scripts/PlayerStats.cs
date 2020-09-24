using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private PlayerController _pcScript;
    
    public float health = 100f;
    public float stamina = 100f;
    public float passiveRegen;
    private float _staminaMax = 100f;
    private float _regenCooldown = 0f;

    private void Start()
    {
        _pcScript = GetComponent<PlayerController>();
    }

    public void TakeDamage()
    {
        
    }
    
    public void DrainStamina()
    {
        //TODO: make the stamina cost a variable probably
        stamina -= 5f * Time.deltaTime;
        _regenCooldown = 2f;
    }

    public void ChunkStamina(float chunkNum)
    {
        stamina -= chunkNum;
        _regenCooldown = 2f;
    }

    public void RegenerateStamina()
    {
        if (_regenCooldown > 0f)
            _regenCooldown -= Time.deltaTime;
        if (stamina < _staminaMax && _regenCooldown <= 0f)
            stamina += passiveRegen * Time.deltaTime;
    }
}
