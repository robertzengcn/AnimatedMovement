using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    PlayerInput _playerInput; //获取用户输入操作用的
    CharacterController _characterController; //角色控制器
    Animator _animator;

    Vector2 _currentMovementInput; //当前的移动输入
    Vector3 _currentMovement;        //当前walk移动
    Vector3 _currentRunMovement;     //当前run移动
    Vector3 _appliedMovement;        //附加跳跃动作后的移动
    bool _isMovementPressed;
    bool _isRunPressed;
    bool _isJumpPressed = false;

    private float _runMultiplier = 3.0f;
    public float _roationFactorPerFrame = 15.0f;
    int _zero = 0;

    float _gravity = -9.8f;
    float _groudedGravity = -.05f;


    int _isWalkingHash;
    int _isRunningHash;
    int _isJumpingHash;
    int _jumpCountHash;

    public float _initialJumpVelocity;    //initial jump velocity
    float _maxJumpHeight = 2.0f;
    float _maxJumpTime = 0.75f;
    bool _isJumping = false;
    //bool _isJumpAnimating = false;
    bool _requireNewJumpPress = false;
    public float RunMultiplier
    {
        get
        {
            return _runMultiplier;
        }
        set
        {
            _runMultiplier=value;
        }
    }
    public bool IsRunPressed
    {
        get
        {
            return _isRunPressed;
        }
        set
        {
            _isRunPressed = value;
        }
    }
    public bool RequireNewJumpPress
    {
        get
        {
            return _requireNewJumpPress;
        }
        set
        {
            _requireNewJumpPress = value;
        }
    }


    public float Gravity
    {
        get
        {
            return _gravity;
        }
        set
        {
            _gravity = value;
        }
    }

    int _jumpCount = 0;
    public Animator Animator
    {
        get
        {
            return _animator;
        }
        set
        {
            _animator = value;
        }
    }

    public CharacterController CharacterController
    {
        get
        {
            return _characterController;
        }
        set
        {
            _characterController = value;
        }
    }


    public float InitialJumpVelocity
    {
        get
        {
            return _initialJumpVelocity;
        }
        set
        {
            _initialJumpVelocity=value;
        }
    }
    public int JumpCount
    {
        get
        {
            return _jumpCount;

        }
        set
        {
            _jumpCount = value;
        }
    }
    public int IsJumpingHash
    {
        get
        {
            return _isJumpingHash;

        }
        set
        {
            _isJumpingHash = value;
        }
    }

    public int JumpCountHash
    {
        get
        {
            return _jumpCountHash;
        }
    }

    public bool IsJumping
    {
        get
        {
            return _isJumping;
        }
        set
        {
            _isJumping = value;
        }
    }
    public Vector2 CurrentMovementInput
    {
        get
        {
            return _currentMovementInput;
        }
        set
        {
            _currentMovementInput = value;
        }
    }


    public bool IsJumpPressed
    {
        get
        {
            return _isJumpPressed;
        }
        set
        {
            _isJumpPressed = value;
        }
    }

    public float CurrentMovementY
    {
        get
        {
            return _currentMovement.y;
        }
        set
        {
            _currentMovement.y = value;
        }
    }
    public float AppliedMovementY
    {
        get
        {
            return _appliedMovement.y;
        }
        set
        {
            _appliedMovement.y = value;
        }
    }
    public float AppliedMovementZ
    {
        get
        {
            return _appliedMovement.z;
        }
        set
        {
            _appliedMovement.z = value;
        }
    }

    public float AppliedMovementX
    {
        get
        {
            return _appliedMovement.x;
        }
        set
        {
            _appliedMovement.x = value;
        }
    }
    public bool IsMovementPressed
    {
        get
        {
            return _isMovementPressed;
        }
        
    }

    public int IsWalkingHash
    {
        get
        {
            return _isWalkingHash;
        }
        set
        {
            _isWalkingHash = value;
        }
    }

    public int IsRunningHash
    {
        get
        {
            return _isRunningHash;
        }
        set
        {
            _isRunningHash = value;
        }
    }


    Dictionary<int, float> _initialJumpVelocities = new Dictionary<int, float>();  //多次跳跃的初始速度
    Dictionary<int, float> _jumpGravities = new Dictionary<int, float>();         //多次跳跃的重力

    Coroutine _currentJumpResetRountine = null;

    public Dictionary<int, float> InitialJumpVelocities
    {
        get
        {
            return _initialJumpVelocities;
        }
        set
        {
            _initialJumpVelocities = value;
        }
    }
    public Dictionary<int, float> JumpGravities
    {
        get
        {
            return _jumpGravities;
        }
        set
        {
            _jumpGravities = value;
        }
    }


    public Coroutine CurrentJumpResetRountine
    {
        get
        {
            return _currentJumpResetRountine;
        }
        set
        {
            _currentJumpResetRountine = value;
        }
    }

    

    public float GroudedGravity
    {
        get
        {
            return _groudedGravity;
        }
        set
        {
            _groudedGravity= value;
        }
    }

    public float CurrentRunMovementY
    {
        get
        {
            return _currentRunMovement.y;
        }
        set
        {
            _currentRunMovement.y= value;
        }
    }

    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    //getters and setters
    public PlayerBaseState CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            _currentState = value;
        }
    }


    void Awake()
    {


        //Debug.Log("awake");
        _playerInput = new PlayerInput();
        if (_playerInput != null)
        {
            Debug.Log("playerInput in awake is not null");
        }
        else
        {
            Debug.Log("playerInput in awake is null");
        }
        _characterController = GetComponent<CharacterController>();
        //set up state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        _playerInput.CharacterControls.Move.started += context =>
        {
            Debug.Log("move start");

            onMovemnetInput(context);

        };
        _playerInput.CharacterControls.Move.canceled += context =>
        {
            Debug.Log("move cancled");

            onMovemnetInput(context);

        };
        _playerInput.CharacterControls.Move.performed += context =>
        {
            Debug.Log("move cancled");

            onMovemnetInput(context);

        };
        _animator = GetComponent<Animator>();
        _playerInput.CharacterControls.Run.started += onRun;
        _playerInput.CharacterControls.Run.canceled += onRun;
        _playerInput.CharacterControls.Jump.started += onJump;
        _playerInput.CharacterControls.Jump.canceled += onJump;
        //playerInput.CharacterControls.Jump.performed += onJump;


        _isJumpingHash = Animator.StringToHash("isJumping");
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _jumpCountHash = Animator.StringToHash("jumpCount");
        SetupJumpVariables();
    }

    void SetupJumpVariables()
    {
        //跳跃公式：
        float timetoApex = _maxJumpTime / 2; //到达顶部的时间
        float firstGravity = (-2 * _maxJumpHeight) / Mathf.Pow(timetoApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timetoApex;
        float secondJumpGravity = (-2 * (_maxJumpHeight + 2)) / Mathf.Pow((timetoApex * 1.25f), 2);
        float secondJumpInitialVelocity = (2 * (_maxJumpHeight + 2)) / (timetoApex * 1.25f);
        float thirdJumpGravity = (-2 * (_maxJumpHeight + 4)) / Mathf.Pow(timetoApex * 1.5f, 2);
        float thirdJumpInitialVelocity = (2 * (_maxJumpHeight + 3)) / Mathf.Pow((timetoApex * 1.5f), 2);
        _initialJumpVelocities.Add(1, _initialJumpVelocity);
        _initialJumpVelocities.Add(2, secondJumpInitialVelocity);
        _initialJumpVelocities.Add(3, thirdJumpInitialVelocity);
        _jumpGravities.Add(0, _gravity);
        _jumpGravities.Add(1, firstGravity);
        _jumpGravities.Add(2, secondJumpGravity);
        _jumpGravities.Add(3, thirdJumpGravity);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //handleAnimation();
        handleRotation();
        _currentState.UpdateStates();
        _characterController.Move(_appliedMovement * Time.deltaTime);

        //if (_isRunPressed)
        //{
        //    _appliedMovement.x = _currentRunMovement.x;
        //    _appliedMovement.z = _currentRunMovement.z;
        //    //characterController.Move(currentRunMovement * Time.deltaTime);
        //}
        //else
        //{
        //    _appliedMovement.x = _currentMovement.x;
        //    _appliedMovement.z = _currentMovement.z;
        //    //characterController.Move(currentMovement * Time.deltaTime);
        //}

        //handleGravity();
        //handleJump();
    }
    void onJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();

        _requireNewJumpPress = false;
        //Debug.Log(isJumpPressed);
    }
    void onRun(InputAction.CallbackContext context)
    {
        //Debug.Log("run start");
        _isRunPressed = context.ReadValueAsButton();
        //if (isRunPressed)
        //{
        //    animator.SetBool(isRunningHash, true);
        //}
        //else
        //{
        //    animator.SetBool(isRunningHash, false);
        //}
        //animator.SetBool("isRunning", true);
    }
    void onMovemnetInput(InputAction.CallbackContext context)
    {
        Debug.Log("move start");

        Debug.Log(context.ReadValue<Vector2>());
        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovement.x = _currentMovementInput.x;
        _currentMovement.z = _currentMovementInput.y;
        _currentRunMovement.x = _currentMovementInput.x * _runMultiplier;
        _currentRunMovement.z = _currentMovementInput.y * _runMultiplier;
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    //void handleAnimation()
    //{
    //    bool isWalking = _animator.GetBool(_isWalkingHash);
    //    bool isRunning = _animator.GetBool(_isRunningHash);
    //    if (_isMovementPressed && !isWalking)
    //    {
    //        _animator.SetBool(_isWalkingHash, true);

    //    }
    //    else if (!_isMovementPressed && isWalking)
    //    {
    //        _animator.SetBool(_isWalkingHash, false);
    //    }
    //    if ((_isMovementPressed && _isRunPressed) && !isRunning)
    //    {
    //        _animator.SetBool(_isRunningHash, true);
    //    }
    //    else if ((!_isMovementPressed || !_isRunPressed) && isRunning)
    //    {
    //        _animator.SetBool(_isRunningHash, false);
    //    }
    //}
    //handle roation problem
    void handleRotation()
    {
        Vector3 positionToLookAt;
        //Quaternion currentRotaion;
        positionToLookAt.x = _currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = _currentMovement.z;
        Quaternion currentRotation = transform.rotation;
        if (_isMovementPressed)
        {
            //look at the direction of the movement
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _roationFactorPerFrame * Time.deltaTime);
        }
        //Quaternion targetRotation= Quaternion.LookRotation(positionToLookAt);
    }

    //void handleGravity()
    //{
    //    _animator.SetBool(_isJumpingHash, false);
    //    bool isFalling = _currentMovement.y <= 0.0f || !_isJumpPressed;
    //    float fallMultiplier = 2.0f;
    //    if (_characterController.isGrounded)
    //    {
    //        //set animator here
    //        if (_isJumpAnimating)
    //        {
    //            _animator.SetBool(_isJumpingHash, false);
    //            _isJumpAnimating = false;
    //            currentJumpResetRountine = StartCoroutine(jumpResetRoutine());
    //            if (_jumpCount == 3)
    //            {
    //                _jumpCount = 0;
    //                _animator.SetInteger(_jumpCountHash, _jumpCount);
    //            }
    //        }
    //        _currentMovement.y = _groudedGravity;
    //        _appliedMovement.y = _groudedGravity;
    //    }
    //    else if (isFalling)
    //    {
    //        Debug.Log("falling");
    //        float previousYVelocity = _currentMovement.y;
    //        _currentMovement.y = _currentMovement.y + (jumpGravities[_jumpCount] * fallMultiplier * Time.deltaTime);
    //        _appliedMovement.y = Mathf.Max((previousYVelocity + _currentMovement.y) * .5f, -20.0f);
    //        //currentMovement.y = currentMovement.y;
    //        //Debug.Log("falling current movement y" + currentMovement.y);
    //        _currentRunMovement.y = _currentMovement.y;
    //    }
    //    else
    //    {
    //        //float groudedGravity = -9.8f;
    //        float previousYVelocity = _currentMovement.y;

    //        _currentMovement.y = _currentMovement.y + (jumpGravities[_jumpCount] * Time.deltaTime);


    //        _appliedMovement.y = (previousYVelocity + _currentMovement.y) * 0.5f;

    //        //currentMovement.y = currentMovement.y;

    //        _currentRunMovement.y = _currentMovement.y;

    //    }
    //}
    //void handleJump()
    //{
    //    Debug.Log("jump count" + _jumpCount);
    //    if (!_isJumping && _characterController.isGrounded && _isJumpPressed)
    //    {
    //        if (_jumpCount < 3 && currentJumpResetRountine != null)
    //        {
    //            StopCoroutine(currentJumpResetRountine);
    //        }
    //        //set animator here
    //        _animator.SetBool(_isJumpingHash, true);
    //        _isJumpAnimating = true;
    //        _isJumping = true;
    //        _jumpCount += 1;
    //        _animator.SetInteger(_isJumpingHash, _jumpCount);
    //        //Debug.Log("initialJumpVelocity var is" + initialJumpVelocity);
    //        _currentMovement.y = initialJumpVelocities[_jumpCount];
    //        //Debug.Log("initialJumpVelocity" + currentMovement.y);
    //        _appliedMovement.y = initialJumpVelocities[_jumpCount];
    //    }
    //    else if (!_isJumpPressed && _isJumping && _characterController.isGrounded)
    //    {
    //        _isJumping = false;
    //    }
    //}
    //IEnumerator jumpResetRoutine()
    //{
    //    //初始化一个线程，N时间后执行
    //    yield return new WaitForSeconds(.5f);
    //    _jumpCount = 0;
    //}
    void OnEnable()
    {
        //Debug.Log("on enable");
        if (_playerInput != null)
        {
            _playerInput.CharacterControls.Enable();
        }
        else
        {
            Debug.Log("playerInput is null");
        }


    }
    private void OnDisable()
    {
        Debug.Log("on disable");
        if (_playerInput != null)
        {
            _playerInput.CharacterControls.Disable();//禁用用户角色控制输入
        }
        else
        {
            Debug.Log("playerInput is null");
        }
    }
}
