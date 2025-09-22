using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SheepIdleState : IdleState
{
    public SheepIdleState(Sheep character,
                          StateMachine stateMachine,
                          CharacterStats _characterStats,
                          Tile _anchorTile) : base(character, stateMachine, _characterStats, _anchorTile) { }
    public override async Task MakeTurn(CancellationToken ct)
    {
        FindTarget();
        if (character.target != null)
        {
            stateMachine.ChangeState(character.battleState);
        }
        while (characterStats.ActionPoints > 0)
        {
            Path path = Pathfinder.FindPath(character.characterTile, RandomTileInRadius(), characterStats.ActionPoints);
            if (path == null)
            {
                Debug.LogError($"{character.name} path equals null");
                return;
            }
            await character.characterMovement.MoveAlongPath(path);
            await Task.Yield();
            if (ct.IsCancellationRequested)
            {
                return;
            }
        }

        character.FinishedTurn = true;
    }
    public override void OnPlayerMadeStep()
    {
        base.OnPlayerMadeStep();
        ChangeStateToBattle();
    }
    public override void OnStep(ref bool needToChangeState)
    {
        base.OnStep(ref needToChangeState);
        if (TargetInDetectZone())
        {
            stateMachine.ChangeState(character.battleState);
            needToChangeState = true;
            return;
        }
        needToChangeState = false;
    }
    private void ChangeStateToBattle()
    {
        if (TargetInDetectZone())
        {
            stateMachine.ChangeState(character.battleState);
        }
    }
}
