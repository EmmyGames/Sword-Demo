using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Components
    public CharacterController cc;
    public Transform cam;
    private float _startCamRotation;
    private Animator _anim;
    private PlayerStats _psScript;

    //Movement
    public float jumpHeight;
    public float walkSpeed;
    public float walkBackMultiplier;
    public float sprintSpeed;
    public float speed;
    private Vector3 _direction;

    private Vector3 _moveDir;
    private float _targetAngle;
    private float _angle;

    public float jumpControlModifier;
    
    public float turnSmoothTime;
    private float _turnSmoothVelocity;

    public bool isSprinting = false;

    //Ground Checks
    private const float Gravity = -9.81f;
    public float groundDistance;
    public Transform groundCheck;
    public LayerMask groundMask;
    private bool _isGrounded;

    //Used for gravity calculation
    private Vector3 _velocity = Vector3.zero;
    
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _psScript = GetComponent<PlayerStats>();
        
        _anim = GetComponent<Animator>();
        _startCamRotation = cam.eulerAngles.y;

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
        Animate();
        ChangeStats();
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
        //shouldMove is set in the animator based on the enter and exit of the attacking animation
        //Make the player not able to move while attacking
        if(_anim.GetBool(_shouldMove))
            PlayerMovement();
    }

    private void PlayerMovement()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (_isGrounded && _velocity.y < 0f)
        {
            //Reset the velocity that it had accumulated while on the ground
            _velocity.y = -2f;
        }
        var moveX = Input.GetAxis("Horizontal");
        var moveZ = Input.GetAxis("Vertical");

        //only change sprinting bool if grounded. Changing sprinting changes the player rotation and speed
        //which should not happen in midair
        if (_isGrounded)
        {
            //condensed if statement. Very cool
            isSprinting = false || Input.GetButton("Sprint");
        }

        //Maintains the properties of GetAxis
        _direction = new Vector3(moveX, 0f, moveZ);
        
        //But going diagonal is not faster
        if (_direction.magnitude > 1f)
        {
            _direction = _direction.normalized;
        }

        //if not grounded (or most likely jumping), change movement
        if (!_isGrounded)
        {
            //Continue to calculate where the player is looking based on their sprint status
            if (!isSprinting)
            {
                _targetAngle = cam.eulerAngles.y - _startCamRotation;
            }
            else
            {
                _targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y - _startCamRotation;
            }
            _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, _angle, 0f);
            
            //Allows the player to have a slight amount of control while in the air determined by jumpControlModifier
            _moveDir += Quaternion.Euler(0f, _targetAngle, 0f) * _direction * (jumpControlModifier * Time.deltaTime);
            cc.Move(speed * Time.deltaTime * _moveDir.normalized);
        }
        //Changes to walking 3rd person mode
        else if (!isSprinting)
        {
            speed = walkSpeed;
            if (_direction.z < 0.01f)
            {
                speed *= walkBackMultiplier;
            }
            //point player at camera
            _targetAngle = cam.eulerAngles.y - _startCamRotation;
            //smooth player rotation
            _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            _moveDir = Quaternion.Euler(0f, _targetAngle, 0f) * _direction;
            
            //put if(direction.magnitude > 0.1f) to make the player not rotate while standing still.
            transform.rotation = Quaternion.Euler(0f, _angle, 0f);
            cc.Move(speed * Time.deltaTime * _moveDir.normalized);
        }
        //Changes to sprinting 3rd person mode
        else if(isSprinting)
        {
            speed = sprintSpeed;
            //point player relative to the camera based on which direction they are moving
            _targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y - _startCamRotation;
            _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            //if the player isn't moving, this will be 0, 0, 0 which is intended
            _moveDir = Quaternion.Euler(0f, _targetAngle, 0f) * _direction;
            
            if (_direction.magnitude >= 0.1f)
            {
                transform.rotation = Quaternion.Euler(0f, _angle, 0f);
                _moveDir = Quaternion.Euler(0f, _targetAngle, 0f) * Vector3.forward;
                cc.Move(speed * Time.deltaTime * _moveDir.normalized);
            }
        }
        
        //Jump Calculations
        //Player can jump smaller amounts by letting go of space
        if (Input.GetButton("Jump") && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * Gravity);
        }
        _velocity.y += Gravity * Time.deltaTime;
        cc.Move(_velocity * Time.deltaTime);
    }
    
    private void Animate()
    {
        _anim.SetBool(_isSprinting, isSprinting && _direction.magnitude >= 0.1f && _isGrounded);
        _anim.SetBool(_isJumping, !_isGrounded);
        _anim.SetBool(_isAttacking, Input.GetButtonDown("Attack") && _isGrounded && _psScript.stamina >= 10f);
        _anim.SetBool(_isAttacking2, Input.GetButtonDown("Heavy Attack") && _isGrounded && _psScript.stamina >= 15f);
        _anim.SetBool(_isBlocking, Input.GetButton("Block") && _isGrounded && _psScript.stamina >= 1f);
        //Blend tree variables
        _anim.SetFloat(_velocityX, _direction.x);
        _anim.SetFloat(_velocityZ, _direction.z);
    }

    private void ChangeStats()
    {
        if (_anim.GetBool(_isAttacking))
        {
            _psScript.ChunkStamina(5f);
        }
        else if (_anim.GetBool(_isAttacking2))
        {
            _psScript.ChunkStamina(10f);
        }
        else if (_anim.GetBool(_isBlocking))
        {
            _psScript.DrainStamina();
        }
        else
        {
            _psScript.RegenerateStamina();
        }
    }
}
