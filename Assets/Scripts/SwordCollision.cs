using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    public Animator _anim;
    
    public float lightDamage;
    public float heavyDamage;
    public float attackDamage;

    private int _isAttacking;
    private int _isAttacking2;

    private void Start()
    {
        _isAttacking = Animator.StringToHash("isAttacking");
        _isAttacking2 = Animator.StringToHash("isAttacking2");
    }

    private void Update()
    {
        if (_anim.GetBool(_isAttacking))
            attackDamage = lightDamage;
        if (_anim.GetBool(_isAttacking2))
            attackDamage = heavyDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the player hits something besides himself
        
        if (other.CompareTag("Shield"))
        {
            attackDamage = 0f;
        }
        else if (other.CompareTag("Sword"))
        {
            attackDamage = 0f;
        }
        else if (other.CompareTag("Player"))
        {
            PlayerStats stats = other.GetComponent<PlayerStats>();
            stats.health -= attackDamage;
        }

        
    }
}
