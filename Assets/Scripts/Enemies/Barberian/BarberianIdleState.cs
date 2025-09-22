using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BarberianIdleState : IdleState
{
    public BarberianIdleState(Barberian character,
                        StateMachine stateMachine,
                        CharacterStats _characterStats,
                        Tile _anchorTile) : base(character, stateMachine, _characterStats, _anchorTile) { }
    public override void OnStep(ref bool needToChangeState)
    {
        base.OnStep(ref needToChangeState);
        if (TargetInAttackZone())
        {
            stateMachine.ChangeState(character.battleState);
            needToChangeState = true;
            return;
        }
        if (TargetInDetectZone())
        {
            character.chaseState.SetTarget(character.target);
            stateMachine.ChangeState(character.chaseState);
            needToChangeState = true;
            return;
        }
        needToChangeState = false;
    }
    public override void OnPlayerMadeStep()
    {
        base.OnPlayerMadeStep();
        if (TargetInAttackZone())
        {
            stateMachine.ChangeState(character.battleState);
            return;
        }
        if (TargetInDetectZone())
        {
            character.chaseState.SetTarget(character.target);
            stateMachine.ChangeState(character.chaseState);
        }
    }
    public override async Task MakeTurn(CancellationToken ct)
    {
        while (characterStats.ActionPoints > 0)
        {
            Path path = Pathfinder.FindPath(character.characterTile, RandomTileInRadius(), characterStats.ActionPoints);
            if (path == null)
            {
                Debug.LogError($"{character.name} path equals null");
                return;
            }
            await character.characterMovement.MoveAlongPath(path);
            if (ct.IsCancellationRequested)
            {
                return;
            }
        }
        character.FinishedTurn = true;
    }
}
