using UnityEngine;

public class FSM
{
    //现在的状态
    public IState CurrentState {  get; private set; }
    
    //切换状态
    public void SwitchState(IState newState)
    {
        CurrentState?.Exit();
        CurrentState=newState;
        CurrentState.Enter();
    }

    //更新状态
    public void Update()
    {
        CurrentState?.Update();
    }
}
