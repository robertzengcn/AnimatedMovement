using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory):base(currentContext, playerStateFactory) 
    {
        IsRootState = true;
        InitializeSubState();
    }
    public override void CheckSwitchStates()
    {
        //Debug.Log("Require new jump is"+ Ctx.RequireNewJumpPress);
        //Debug.Log("is jump press is" + Ctx.IsJumpPressed);
        //throw new System.NotImplementedException();
        if (Ctx.IsJumpPressed&&!Ctx.RequireNewJumpPress)
        {
            SwitchStates(Factory.Jump());
        }
    }

    public override void EnterState()
    {
        //Debug.Log("ENTERING GROUNDED STATE");
        //throw new System.NotImplementedException();
        //Debug.Log("HELLO FROM THE GROUNDED STATE");
        Ctx.CurrentMovementY = Ctx.GroudedGravity;
        Ctx.AppliedMovementY = Ctx.GroudedGravity;
    }

    public override void ExitState()
    {
       
    }

    public override void InitializeSubState()
    {
       if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }else if ((Ctx.IsMovementPressed && !Ctx.IsRunPressed))
        {
            //animator.SetBool(isRunningHash, true);
            SetSubState(Factory.Walk());
        }
        else
        {
            //Debug.Log("movement press is"+Ctx.IsMovementPressed);
            //Debug.Log("run press is" + Ctx.IsRunPressed);
            //Debug.Log("init run state factory");

            SetSubState(Factory.Run());
        }
        
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

  
}
