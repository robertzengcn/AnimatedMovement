using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }
    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();
        if(!Ctx.IsMovementPressed)
        {
            SwitchStates(Factory.Idle());
        }else if(Ctx.IsMovementPressed&&Ctx.IsRunPressed)
        {
            SwitchStates(Factory.Run());
        }
    }

    public override void EnterState()
    {
        //Debug.Log("HELLO FROM THE WALK STATE");
        //throw new System.NotImplementedException();
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        //throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x;
        Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y;
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
