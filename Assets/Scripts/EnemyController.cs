using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //Components
    private Movement _movement;
    private Animator _anim;

    //Movement
    private Vector3 _direction;
    private Vector3 _lastDirection;
    public bool isSprinting = false;
    private float _startRotation;
    private bool _jumping = false;
    
    //Ground Checks
    public float groundDistance;
    public Transform groundCheck;
    public LayerMask groundMask;
    private bool _isGrounded;
    
    //Animator parameters
    private int _isSprinting;
    private int _isJumping;
    private int _isAttacking;
    private int _isAttacking2;
    private int _isBlocking;
    private int _shouldMove;
    private int _velocityX;
    private int _velocityZ;
    
    private void Start()
    {
        _startRotation = transform.eulerAngles.y;
        
        _movement = GetComponent<Movement>();
        _anim = GetComponent<Animator>();
        
        _isSprinting = Animator.StringToHash("isSprinting");
        _isJumping = Animator.StringToHash("isJumping");
        _isAttacking = Animator.StringToHash("isAttacking");
        _isAttacking2 = Animator.StringToHash("isAttacking2");
        _isBlocking = Animator.StringToHash("isBlocking");
        _shouldMove = Animator.StringToHash("shouldMove");
        _velocityX = Animator.StringToHash("velocityX");
        _velocityZ = Animator.StringToHash("velocityZ");
    }

    private void Update()
    {
        if (!PauseMenu.IsPaused)
        {
            Animate();
        }
    }

    private void FixedUpdate()
    {
        EnemyMovement();
    }

    private void EnemyMovement()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (_anim.GetBool(_shouldMove) && !PauseMenu.IsPaused)
            _movement.EntityMovement(_isGrounded, isSprinting, _jumping, transform, _startRotation, _direction);
    }
    
    private void Animate()
    {
        _anim.SetBool(_isSprinting, isSprinting && _direction.magnitude >= 0.1f && _isGrounded);
        _anim.SetBool(_isJumping, !_isGrounded);
        /*
        _anim.SetBool(_isAttacking, Input.GetButtonDown("Attack") && _isGrounded && _psScript.stamina >= 10f && _anim.GetBool(_shouldMove));
        _anim.SetBool(_isAttacking2, Input.GetButtonDown("Heavy Attack") && _isGrounded && _psScript.stamina >= 15f && _anim.GetBool(_shouldMove));
        _anim.SetBool(_isBlocking, Input.GetButton("Block") && _isGrounded && _psScript.stamina >= 1f);
        */
        //Blend tree variables
        _anim.SetFloat(_velocityX, _direction.x);
        _anim.SetFloat(_velocityZ, _direction.z);
    }
}
