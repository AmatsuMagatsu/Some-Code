using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine {

    private IState currentState;
    private IState prevState;

    public void ChangeState(IState newState)
    {
        if (this.currentState != null)
        {
            this.currentState.Exit();
        }
        this.prevState = this.currentState;

        this.currentState = newState;
        this.currentState.Enter();
    }
    public void ExecuteStateUpdate()
    {
        var runningState = this.currentState;
        if(runningState != null)
        {
            this.currentState.Execute();
        }
        
    }

    public void SwitchToPrevState()
    {
        this.currentState.Exit();
        this.currentState = this.prevState;
        this.currentState.Enter();
    }
}
