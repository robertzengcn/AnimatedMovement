using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }
    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();
        if (!Ctx.IsRunPressed)
        {
            SwitchStates(Factory.Idle());
        }else if(Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SwitchStates(Factory.Walk());
        }
    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, true);
    }

    public override void ExitState()
    {
        //Debug.Log("EXITING RUN STATE");
        //throw new System.NotImplementedException();
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x*Ctx.RunMultiplier;
        Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y* Ctx.RunMultiplier;
        //throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
