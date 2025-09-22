using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public State CurrentState { get; set; }
    public List<State> AllStates = new List<State>();

    public void Initiallize(State startingState)
    {
        CurrentState = startingState;
        CurrentState.EnterState();
/*        Debug.Log(CurrentEnemyState);*/
    }

    public void ChangeState(State newState)
    {
        CurrentState.ExitState();
        CurrentState = newState;
        CurrentState.EnterState();
/*        Debug.Log(CurrentEnemyState);*/
    }

    public void AddState(State state)
    {
        AllStates.Add(state);
    }
}
