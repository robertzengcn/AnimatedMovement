using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    

    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        //SwitchStates(Factory.Grounded());
    }

    IEnumerator IJumpResetRoutine()
    {
        //初始化一个线程，N时间后执行
        yield return new WaitForSeconds(.5f);
       Ctx.JumpCount = 0;
    }
    public override void CheckSwitchStates()
    {
        if (Ctx.CharacterController.isGrounded)
        {
            Debug.Log("it is at ground");
           Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
            Ctx.RequireNewJumpPress = false;
            //Debug.Log("switch to ground state");
            SwitchStates(Factory.Grounded()); 
        }
    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
        HandleJump();
    }

    public override void ExitState()
    {
        Debug.Log("EXITING JUMP STATE");
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
        //Ctx.IsJumpAnimating = false;
        if (Ctx.IsJumpPressed)
        {
           Ctx.RequireNewJumpPress = true;
        }
        
       Ctx.CurrentJumpResetRountine =Ctx.StartCoroutine(IJumpResetRoutine());
        if (Ctx.JumpCount == 3)
        {
           Ctx.JumpCount = 0;
           Ctx.Animator.SetInteger(Ctx.JumpCountHash,Ctx.JumpCount);
        }
    }

    public override void InitializeSubState()
    {
        //throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        HanldeGravity();
    }

   void HandleJump()
    {
        if (Ctx.JumpCount < 3 &&Ctx.CurrentJumpResetRountine != null)
        {
           Ctx.StopCoroutine(Ctx.CurrentJumpResetRountine);
        }
        //set animator here
       Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);
        //Ctx.IsJumpAnimating = true;
       Ctx.IsJumping = true;
       Ctx.JumpCount += 1;
       Ctx.Animator.SetInteger(Ctx.JumpCountHash,Ctx.JumpCount);
        //Debug.Log("initialJumpVelocity var is" +Ctx.InitialJumpVelocity);
       Ctx.CurrentMovementY =Ctx.InitialJumpVelocities[Ctx.JumpCount];
        //Debug.Log("initialJumpVelocity" + currentMovement.y);
       Ctx.AppliedMovementY =Ctx.InitialJumpVelocities[Ctx.JumpCount];
    }

    void HanldeGravity()
    {
        bool isFalling =Ctx.CurrentMovementY <= 0.0f || !Ctx.IsJumpPressed;
        float fallMultiplier = 2.0f;
        if (isFalling)
        {
            //Debug.Log("falling");
            float previousYVelocity =Ctx.CurrentMovementY;
           Ctx.CurrentMovementY =Ctx.CurrentMovementY + (Ctx.JumpGravities[Ctx.JumpCount] * fallMultiplier * Time.deltaTime);
           Ctx.AppliedMovementY = Mathf.Max((previousYVelocity +Ctx.CurrentMovementY) * .5f, -20.0f);
            //currentMovement.y = currentMovement.y;
            //Debug.Log("falling current movement y" + currentMovement.y);
            //Ctx.CurrentMovementY = currentMovement.y;
        }
        else
        {
            //float groudedGravity = -9.8f;
            float previousYVelocity =Ctx.CurrentMovementY;

           Ctx.CurrentMovementY =Ctx.CurrentMovementY + (Ctx.JumpGravities[Ctx.JumpCount] * Time.deltaTime);


           Ctx.AppliedMovementY = (previousYVelocity +Ctx.CurrentMovementY) * 0.5f;

            //currentMovement.y = currentMovement.y;

           Ctx.CurrentRunMovementY =Ctx.CurrentMovementY;

        }
    }   
}
