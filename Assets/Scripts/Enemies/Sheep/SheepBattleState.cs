using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SheepBattleState : BattleState
{
    public SheepBattleState(Sheep character,
                            StateMachine stateMachine,
                            CharacterStats _characterStats) :base(character, stateMachine, _characterStats)
    {
        characterStats = _characterStats;
    }
    private Tile NextTileFromEnemy()
    {
        List<Tile> reachable = Pathfinder.ReachableTiles(character.characterTile, characterStats.ActionPoints);
        Vector3 targetPos = character.target.transform.position;
        Tile farestTile = null;
        if (reachable.Count != 0)
        {
            farestTile = reachable[0];
        }
        foreach (Tile tile in reachable)
        {
            float curDistToPlayer = (farestTile.transform.position - targetPos).magnitude;
            float distToPlayer = (tile.transform.position - targetPos).magnitude;
            if (curDistToPlayer < distToPlayer)
            {
                farestTile = tile;
            }
        }
        return farestTile;
    }
    public override async Task MakeTurn(CancellationToken ct)
    {
        FindTarget();
        if (character.target == null)
        {
            stateMachine.ChangeState(character.idleState);
        }
        while (characterStats.ActionPoints > 0) 
        {
            Tile nextTile = NextTileFromEnemy();
            if (nextTile == null)
            {
                //do not move
                //play animation
                return;
            }
            Path path = Pathfinder.FindPath(character.characterTile, nextTile, characterStats.ActionPoints);
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
        ChangeStateToIdle();
    }
    public override void OnStep(ref bool needToChangeState)
    {
        base.OnStep(ref needToChangeState);
        if (TargetOutOfDetectZone())
        {
            stateMachine.ChangeState(character.idleState);
            needToChangeState = true;
            character.target = null;
            return;
        }
        needToChangeState = false;
    }

    private void ChangeStateToIdle()
    {
        if (TargetOutOfDetectZone())
        {
            stateMachine.ChangeState(character.idleState);
        }
    }
}
