using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {

    }

    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();
        if(Ctx.IsMovementPressed&&Ctx.IsRunPressed)
        {
            SwitchStates(Factory.Run());
        }else if (Ctx.IsMovementPressed)
        {
            SwitchStates(Factory.Walk());
        }
    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
        Ctx.AppliedMovementY = 0;
        Ctx.AppliedMovementZ = 0;
    }

    public override void ExitState()
    {
        //throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        //throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
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
