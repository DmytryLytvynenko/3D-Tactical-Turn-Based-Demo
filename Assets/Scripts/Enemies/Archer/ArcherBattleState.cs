using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ArcherBattleState : BattleState
{
    private SkillAgent agent;
    private Archer archer;
    public ArcherBattleState(Archer character,
                     StateMachine stateMachine,
                     CharacterStats _characterStats,
                     SkillAgent agent) : base(character, stateMachine, _characterStats) 
    {
        archer = character;
        this.agent = agent;
    }
    public override void EnterState()
    {
        base.EnterState();
    }
    public override void ExitState()
    {
        base.ExitState();
    }
    public override void OnStep(ref bool needToChangeState)
    {
        base.OnStep(ref needToChangeState);
        if (TargetOutOfAttackZone())
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
        if (TargetOutOfAttackZone())
        {
            character.chaseState.SetTarget(character.target);
            stateMachine.ChangeState(character.chaseState);
        }
    }
    public override async Task MakeTurn(CancellationToken ct)
    {
        FindTarget();
        if (character.target == null)
        {
            stateMachine.ChangeState(character.idleState);
            return;
        }
        else
        {
            if (TargetOutOfAttackZone())
            {
                stateMachine.ChangeState(character.chaseState);
                return;
            }
        }
        if (TargetIsTooClose())
        {
            Tile nextTile = NextTileFromEnemy();
            if (nextTile == null)
            {
                return;
            }
            Path path = Pathfinder.FindPath(character.characterTile, nextTile, characterStats.ActionPoints);
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
        while (characterStats.ActionPoints > 0)
        {
            await RotateToTarget();
            await Task.Delay(500);// time before attack / start animation
            await Attack();
            if (character.target.IsDead && characterStats.ActionPoints > 0)
            {
                character.target = null;
                FindTarget();
                if (character.target == null)
                {
                    stateMachine.ChangeState(character.idleState);
                    return;
                }
                else
                {
                    if (TargetOutOfAttackZone())
                    {
                        stateMachine.ChangeState(character.chaseState);
                        return;
                    }
                }
                if (TargetIsTooClose())
                {
                    Tile nextTile = NextTileFromEnemy();
                    if (nextTile == null)
                    {
                        return;
                    }
                    Path path = Pathfinder.FindPath(character.characterTile, nextTile, characterStats.ActionPoints);
                    if (path == null)
                    {
                        Debug.LogError($"{character.name} path equals null");
                        break;
                    }
                    await character.characterMovement.MoveAlongPath(path);
                    if (ct.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
            if (ct.IsCancellationRequested)
            {
                return;
            }
        }

        character.FinishedTurn = true; 
    }
    private async Task Attack()
    {
        character.animationController.SetTrigger(CharacterAnimParameters.RangeAttack);
        await Task.Delay(100);
        SkillParameters parameters = new SkillParameters(character.target, archer.shootPoint);
        await agent.UseSkill(SkillName.ArcherBowShot, parameters);
        Debug.Log("Archer attack");
        await Task.Delay(1000);// time between attacks
    }
    private async Task RotateToTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(character.transform.position.DirectionTo(character.target.transform.position).Flat(), Vector3.up);
        while (Math.Round(character.characterVisual.rotation.eulerAngles.y) != Math.Round(targetRotation.eulerAngles.y))
        {
            float t = Mathf.Clamp(Time.deltaTime * 4f, 0f, 0.99f);
            character.characterVisual.rotation = Quaternion.Lerp(character.characterVisual.rotation, targetRotation, t);
            character.characterVisual.rotation = Quaternion.Euler(0, character.characterVisual.localEulerAngles.y, 0f);

            await Task.Yield();
        }
    }
    private bool TargetIsTooClose()
    {
        if (character.target == null)
        {
            return false;
        }
        Vector2 characterPos = new Vector2(character.transform.position.x, character.transform.position.z);
        Vector2 tragetPos = new Vector2(character.target.transform.position.x, character.target.transform.position.z);

        return (tragetPos - characterPos).magnitude < 1.1f;
    }
    private Tile NextTileFromEnemy()
    {
        List<Tile> reachable = Pathfinder.ReachableTiles(character.characterTile, 1);
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
}
