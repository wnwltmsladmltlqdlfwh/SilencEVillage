using Unity.Collections;
using UnityEngine;

public class FSM
{
    public FSM (BaseState initialState)
    {
        currentState = initialState;
        ChangeState(currentState);
    }

    private BaseState currentState;
    public BaseState CurrentState => currentState;

    public void ChangeState(BaseState nextState)
    {
        if(nextState == currentState)
            return;
        
        if(currentState != null)
            currentState?.Exit();
        
        currentState = nextState;
        currentState?.Enter();
    }

    public void UpdateState()
    {
        if(currentState != null)
            currentState?.Update();
    }
}
