using System.Threading;
using System.Threading.Tasks;

public class BarberianChaseState : ChaseState
{
    public BarberianChaseState(Barberian character,
                            StateMachine stateMachine,
                            CharacterStats _characterStats) : base(character, stateMachine, _characterStats) { }
    public override async Task MakeTurn(CancellationToken ct)
    {
        if (characterStats.ChaseTurns == 0)
        {
            characterStats.RestoreChaseTurns();
            if (TargetOutOfDetectZone())
            {
                character.target = null;
                stateMachine.ChangeState(character.idleState);
            }
        }

        Path pathToPlayer = Pathfinder.FindPath(character.characterTile, character.target.characterTile);
        if (pathToPlayer == null)
        {
            if (TargetOutOfDetectZone())
            {
                stateMachine.ChangeState(character.idleState);
            }
            return;
        }
        pathToPlayer.RemoveAt(pathToPlayer.Length - 1);
        if (pathToPlayer.Length - 1 > characterStats.ActionPoints)
        {
            pathToPlayer.RemoveRange(characterStats.ActionPoints + 1, pathToPlayer.Length - 1 - characterStats.ActionPoints);
        }

        if (pathToPlayer == null) return;

        await character.characterMovement.MoveAlongPath(pathToPlayer);
        if (ct.IsCancellationRequested)
        {
            return;
        }
        characterStats.ChaseTurns--;
        character.FinishedTurn = true;
    }
    public override void OnPlayerMadeStep()
    {
        base.OnPlayerMadeStep();
        if (TargetInAttackZone())
        {
            stateMachine.ChangeState(character.battleState);
        }
        if (TargetOutOfDetectZone())
        {
            stateMachine.ChangeState(character.idleState);
        }
    }
    public override void OnStep(ref bool needToChangeState)
    {
        base.OnStep(ref needToChangeState);
        if (TargetInAttackZone())
        {
            stateMachine.ChangeState(character.battleState);
            needToChangeState = true;
            return;
        }
        if (TargetOutOfDetectZone())
        {
            stateMachine.ChangeState(character.idleState);
            needToChangeState = true;
            return;
        }
        needToChangeState = false;
    }
}
