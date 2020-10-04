using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    //Components
    private Movement _movement;
    private Animator _anim;
    private PlayerStats _psScript;
    public Transform playerTransform;

    //Movement
    private Vector3 _direction;
    private Vector3 _lastDirection;
    public bool isSprinting = false;
    private float _startRotation;
    private bool _jumping = false;
    public float sprintRadius;
    public float attackRadius;
    
    
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
    
    //State
    private enum State{Idle, Attack, HeavyAttack, Block, Move}

    private float _distance;

    private State state;
    
    private void Start()
    {
        _startRotation = transform.eulerAngles.y;
        
        _movement = GetComponent<Movement>();
        _anim = GetComponent<Animator>();
        _psScript = GetComponent<PlayerStats>();
        
        _isSprinting = Animator.StringToHash("isSprinting");
        _isJumping = Animator.StringToHash("isJumping");
        _isAttacking = Animator.StringToHash("isAttacking");
        _isAttacking2 = Animator.StringToHash("isAttacking2");
        _isBlocking = Animator.StringToHash("isBlocking");
        _shouldMove = Animator.StringToHash("shouldMove");
        _velocityX = Animator.StringToHash("velocityX");
        _velocityZ = Animator.StringToHash("velocityZ");
        
        StartCoroutine(CalculateMove());
    }

    private void Update()
    {
        if (!PauseMenu.IsPaused)
        {
            Animate();
            ChangeStats();
        }
    }

    private void FixedUpdate()
    {
        EnemyDecision();
        EnemyMovement();
    }

    private void EnemyMovement()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (_anim.GetBool(_shouldMove) && !PauseMenu.IsPaused)
            _movement.EntityMovement(_isGrounded, isSprinting, _jumping, transform, _startRotation, _direction);
    }

    private void EnemyDecision()
    {
        float deltaX = playerTransform.position.x - transform.position.x;
        float deltaZ = playerTransform.position.z - transform.position.z;
        float theta = Mathf.Atan2(deltaX, deltaZ) * Mathf.Rad2Deg; //TODO: add random offset (but don't update it that often)
        transform.rotation = Quaternion.Euler(0f, theta, 0f);
        _distance = Vector3.Distance(playerTransform.position, transform.position);

        if (_distance < attackRadius && _distance > 1f && state != State.Attack && state != State.HeavyAttack)
        {
            state = (State)Random.Range(1,4);
        }
    }

    IEnumerator CalculateMove()
    {
        while (true)
        {
            isSprinting  = _distance > sprintRadius;
            
            if (isSprinting)
                _direction = Vector3.forward;
            else if (_distance > 1f && _distance < sprintRadius)
                _direction = new Vector3(Random.Range(-1, 2), 0f, Random.Range(0, 2)).normalized;
            else if(_distance < 1f)
                _direction = new Vector3(Random.Range(-1, 2), 0f, -1).normalized;
            yield return new WaitForSeconds(0.25f);
        }
    }
    
    private void Animate()
    {
        _anim.SetBool(_isSprinting, isSprinting && _direction.magnitude >= 0.1f && _isGrounded);
        _anim.SetBool(_isJumping, !_isGrounded);
        
        _anim.SetBool(_isAttacking, state == State.Attack && _isGrounded && _psScript.stamina >= 10f && _anim.GetBool(_shouldMove));
        _anim.SetBool(_isAttacking2, state == State.HeavyAttack && _isGrounded && _psScript.stamina >= 15f && _anim.GetBool(_shouldMove));
        if (_anim.GetBool(_isAttacking) || _anim.GetBool(_isAttacking2))
            state = State.Idle;
        _anim.SetBool(_isBlocking, state == State.Block && _isGrounded && _psScript.stamina >= 1f);
        
        //Blend tree variables
        _anim.SetFloat(_velocityX, _direction.x);
        _anim.SetFloat(_velocityZ, _direction.z);
    }
    
    private void ChangeStats()
    {
        if (_anim.GetBool(_isAttacking))
            _psScript.ChunkStamina(5f);
        else if (_anim.GetBool(_isAttacking2))
            _psScript.ChunkStamina(10f);
        else if (_anim.GetBool(_isBlocking))
            _psScript.DrainStamina();
        else
            _psScript.RegenerateStamina();
    }
}
