using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{

    PlayerInput playerInput; //获取用户输入操作用的
    CharacterController characterController; //角色控制器
    Animator animator;

    Vector2 currentMovementInput; //当前的移动输入
    Vector3 currentMovement;        //当前walk移动
    Vector3 currentRunMovement;     //当前run移动
    Vector3 appliedMovement;        //附加跳跃动作后的移动
    bool isMovementPressed;
    bool isRunPressed;
    bool isJumpPressed=false;

    public float runMultiplier=3.0f;
    public float roationFactorPerFrame=15.0f;
    int zero = 0;

    float gravity = -9.8f;
    float groudedGravity = -.05f;
    

    int isWalkingHash;
    int isRunningHash;
    int isJumpingHash;
    int jumpCountHash;

    public float initialJumpVelocity;    //initial jump velocity
    float maxJumpHeight=2.0f;
    float maxJumpTime=0.75f;
    bool isJumping = false;
    bool isJumpAnimating = false;

    int jumpCount = 0;
    Dictionary<int, float> initialJumpVelocities = new Dictionary<int, float>();  //多次跳跃的初始速度
    Dictionary<int, float> jumpGravities = new Dictionary<int, float>();         //多次跳跃的重力

    Coroutine currentJumpResetRountine = null;
    void Awake()
    {


        //Debug.Log("awake");
        playerInput = new PlayerInput();
        if(playerInput != null)
        {
            Debug.Log("playerInput in awake is not null");
        }
        else
        {
            Debug.Log("playerInput in awake is null");
        }
        characterController=GetComponent<CharacterController>();
        playerInput.CharacterControls.Move.started += context => {
            Debug.Log("move start");

            onMovemnetInput(context);

        };
        playerInput.CharacterControls.Move.canceled += context => {
            Debug.Log("move cancled");

            onMovemnetInput(context);

        };
        playerInput.CharacterControls.Move.performed += context => {
            Debug.Log("move cancled");

            onMovemnetInput(context);

        };
        animator = GetComponent<Animator>();
        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;
        playerInput.CharacterControls.Jump.started+= onJump;
        playerInput.CharacterControls.Jump.canceled += onJump;
        //playerInput.CharacterControls.Jump.performed += onJump;


        isJumpingHash= Animator.StringToHash("isJumping");
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        jumpCountHash = Animator.StringToHash("jumpCount");
        setupJumpVariables();
    }
    void handleJump() {
        Debug.Log("jump count"+jumpCount);
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            if (jumpCount < 3 && currentJumpResetRountine != null)
            {
                StopCoroutine(currentJumpResetRountine);
            }
            //set animator here
            animator.SetBool(isJumpingHash, true);
            isJumpAnimating = true;
            isJumping = true;
            jumpCount += 1;
            animator.SetInteger(jumpCountHash, jumpCount);
            Debug.Log("initialJumpVelocity var is" + initialJumpVelocity);
            currentMovement.y = initialJumpVelocities[jumpCount];
            Debug.Log("initialJumpVelocity"+ currentMovement.y);
            appliedMovement.y = initialJumpVelocities[jumpCount];
        }else if (!isJumpPressed&&isJumping&&characterController.isGrounded)
        {
            isJumping = false;
        }
    }
    void setupJumpVariables()
    {
        //跳跃公式：
        float timetoApex = maxJumpTime / 2; //到达顶部的时间
        gravity=(-2*maxJumpHeight)/ Mathf.Pow(timetoApex,2);
        initialJumpVelocity=(2*maxJumpHeight) / timetoApex;
        float secondJumpGravity = (-2 * (maxJumpHeight + 2)) / Mathf.Pow((timetoApex * 1.25f), 2);
        float secondJumpInitialVelocity = (2 * (maxJumpHeight + 2)) / (timetoApex * 1.25f);
        float thirdJumpGravity=(-2*(maxJumpHeight+4))/Mathf.Pow(timetoApex * 1.5f, 2);
        float thirdJumpInitialVelocity = (2 * (maxJumpHeight + 3)) / Mathf.Pow((timetoApex * 1.5f),2);
        initialJumpVelocities.Add(1, initialJumpVelocity);
        initialJumpVelocities.Add(2, secondJumpInitialVelocity);
        initialJumpVelocities.Add(3, thirdJumpInitialVelocity);
        jumpGravities.Add(0, gravity);
        jumpGravities.Add(1, gravity);
        jumpGravities.Add(2, secondJumpGravity);
        jumpGravities.Add(3, thirdJumpGravity);
    }
    IEnumerator jumpResetRoutine()
    {
        //初始化一个线程，N时间后执行
        yield return new WaitForSeconds(.5f);
        jumpCount = 0;
    }
    void onJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
        //Debug.Log(isJumpPressed);
    }
    void onRun(InputAction.CallbackContext context)
    {
        Debug.Log("run start");
        isRunPressed = context.ReadValueAsButton();
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
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void handleAnimation()
    {
        bool isWalking=animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        if (isMovementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);

        }else if (!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }
        if ((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }else if ((!isMovementPressed||!isRunPressed)&&isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }
    }
    //handle roation problem
    void handleRotation()
    {
        Vector3 positionToLookAt;
        //Quaternion currentRotaion;
        positionToLookAt.x= currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;
        Quaternion currentRotation = transform.rotation;
        if(isMovementPressed)
        {
            //look at the direction of the movement
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, roationFactorPerFrame*Time.deltaTime);
        }
        //Quaternion targetRotation= Quaternion.LookRotation(positionToLookAt);
    }

    void handleGravity()
    {
        animator.SetBool(isJumpingHash, false);
        bool isFalling = currentMovement.y <= 0.0f||!isJumpPressed;
        float fallMultiplier = 2.0f;
        if (characterController.isGrounded)
        {
            //set animator here
            if (isJumpAnimating)
            {
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
                currentJumpResetRountine=StartCoroutine(jumpResetRoutine());
                if (jumpCount == 3)
                {
                    jumpCount = 0;
                    animator.SetInteger(jumpCountHash, jumpCount);
                }
            }
            currentMovement.y = groudedGravity;
            appliedMovement.y = groudedGravity;
        }
        else if (isFalling)
        {
            Debug.Log("falling");
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (jumpGravities[jumpCount] * fallMultiplier * Time.deltaTime);
            appliedMovement.y = Mathf.Max((previousYVelocity + currentMovement.y) * .5f,-20.0f);
            //currentMovement.y = currentMovement.y;
            //Debug.Log("falling current movement y" + currentMovement.y);
            currentRunMovement.y = currentMovement.y;
        }
        else
        {
            //float groudedGravity = -9.8f;
            float previousYVelocity = currentMovement.y;
            
            currentMovement.y = currentMovement.y + (jumpGravities[jumpCount] * Time.deltaTime);
            

            appliedMovement.y = (previousYVelocity + currentMovement.y) * 0.5f;
           
            //currentMovement.y = currentMovement.y;
           
            currentRunMovement.y = currentMovement.y;
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        handleAnimation();
        handleRotation();
        
        if (isRunPressed)
        {
            appliedMovement.x = currentRunMovement.x;
            appliedMovement.z = currentRunMovement.z;
            //characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            appliedMovement.x= currentMovement.x;
            appliedMovement.z = currentMovement.z;
            //characterController.Move(currentMovement * Time.deltaTime);
        }
        characterController.Move(appliedMovement * Time.deltaTime);
        handleGravity();
        handleJump();
        //if (characterController.isGrounded)
        //{
        //    //Debug.Log("user at ground");
        //}
        //else
        //{
        //    //Debug.Log("user is not at ground");
        //}
       
    }
    void OnEnable()
    {
        Debug.Log("on enable");
        if(playerInput != null)
        {
            playerInput.CharacterControls.Enable();
        }
        else
        {
            Debug.Log("playerInput is null");
        }
       

    }
    private void OnDisable()
    {
        Debug.Log("on disable");
        if (playerInput != null)
        {
            playerInput.CharacterControls.Disable();//禁用用户角色控制输入
        }
        else
        {
            Debug.Log("playerInput is null");
        }
    }

}
