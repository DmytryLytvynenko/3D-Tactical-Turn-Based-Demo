using UnityEngine;

public class Barberian : Enemy
{
    #region member fields
    [SerializeField] private SkillAgent agent;
    private Tile anchorTile;
    public Transform shootPoint;
    #endregion

    protected override void Awake()
    {
        base.Awake();

    }
    protected override void Start()
    {
        base.Start();
        ReferanceContainer.CharacterManager.AddCharacterToActive(this);
        anchorTile = characterTile;

        idleState = new BarberianIdleState(this, stateMachine, characterStats, anchorTile);
        battleState = new BarberianBattleState(this, stateMachine, characterStats, agent);
        chaseState = new BarberianChaseState(this, stateMachine, characterStats);

        switch (startState)
        {
            case StateType.BattleState:
                stateMachine.Initiallize(battleState);
                break;
            case StateType.IdleState:
                stateMachine.Initiallize(idleState);
                break;
            case StateType.ChaseState:
                stateMachine.Initiallize(chaseState);
                break;
            default:
                break;
        }
        bool needToChangeState = false;
        stateMachine.CurrentState.FindTarget();
        stateMachine.CurrentState.OnStep(ref needToChangeState);
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
