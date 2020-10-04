using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Components
    private Movement _movement;
    private Animator _anim;
    private PlayerStats _psScript;
    public Transform cam;

    //Movement
    private Vector3 _direction;
    private Vector3 _lastDirection;
    public bool isSprinting = false;
    private float _startCamRotation;
    
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
        _startCamRotation = cam.eulerAngles.y;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
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
    }

    private void Update()
    {
        if (!PauseMenu.IsPaused)
        {
            Animate();
            ChangeStats();
        }
        
        //Make sure nothing else goes below this
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void FixedUpdate()
    {
        PlayerMovement();
        _direction = _movement.GetDirection();
    }

    private void PlayerMovement()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //only change sprinting bool if grounded. Changing sprinting changes the player rotation and speed
        //which should not happen in midair
        if (_isGrounded)
            isSprinting = false || Input.GetButton("Sprint");

        var moveX = Input.GetAxisRaw("Horizontal");
        var moveZ = Input.GetAxisRaw("Vertical");
        _direction = new Vector3(moveX, 0f, moveZ).normalized;

        //shouldMove is set in the animator based on the enter and exit of the attacking animation
        //Make the player not able to move while attacking
        if (!_anim.GetBool(_shouldMove) || PauseMenu.IsPaused)
            _direction = Vector3.zero;
        _movement.EntityMovement(_isGrounded, isSprinting, Input.GetButton("Jump"), cam, _startCamRotation, _direction);
    }
    
    private void Animate()
    {
        _anim.SetBool(_isSprinting, isSprinting && _direction.magnitude >= 0.1f && _isGrounded);
        _anim.SetBool(_isJumping, !_isGrounded);
        _anim.SetBool(_isAttacking, Input.GetButtonDown("Attack") && _isGrounded && _psScript.stamina >= 10f && _anim.GetBool(_shouldMove));
        _anim.SetBool(_isAttacking2, Input.GetButtonDown("Heavy Attack") && _isGrounded && _psScript.stamina >= 15f && _anim.GetBool(_shouldMove));
        _anim.SetBool(_isBlocking, Input.GetButton("Block") && _isGrounded && _psScript.stamina >= 1f);
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
        //Don't regen stamina while sprinting but don't take any away either
        else if (_anim.GetBool(_isSprinting))
            _psScript.ChunkStamina(0f);
        else
            _psScript.RegenerateStamina();
    }
}
