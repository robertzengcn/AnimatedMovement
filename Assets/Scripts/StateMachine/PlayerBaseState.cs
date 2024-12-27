using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
{
    private bool _isRootState = false;
    private PlayerStateMachine _ctx;
    private PlayerStateFactory _factory;
    protected PlayerBaseState _currentSubState;
    protected PlayerBaseState _currentSuperState;
    protected bool IsRootState { set { _isRootState=value; } }
    protected PlayerStateMachine Ctx
    {
        get
        {
            return _ctx;

        }
    }
    protected PlayerStateFactory Factory
    {
        get
        {
            return _factory;
        }
    }
    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState != null)
        {
            _currentSubState.UpdateState();
        }
    }
    public void ExitStates()
    {
        ExitState();
        if(_currentSubState != null)
        {
            _currentSubState.ExitState();
        }
    }

    protected void SwitchStates(PlayerBaseState newState)
    {
        //current state exits state
        ExitStates();
        newState.EnterState();
        if (_isRootState)
        {
            _ctx.CurrentState = newState;
        }else if(_currentSuperState != null)
        {
            //Debug.Log("set sub state");
            _currentSuperState.SetSubState(newState);
        }
        

    }

    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }

}
