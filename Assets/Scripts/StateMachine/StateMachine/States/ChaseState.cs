using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ChaseState : State
{
    public ChaseState(Enemy character,
                      StateMachine stateMachine,
                      CharacterStats _characterStats) : base(character, stateMachine, _characterStats) { }
    public override Task MakeTurn(CancellationToken ct)
    {
        return Task.CompletedTask;
    }
    protected virtual bool TargetInAttackZone()
    {
        if (character.target == null)
        {
            return false;
        }
        Vector2 characterPos = new Vector2(character.transform.position.x, character.transform.position.z);
        Vector2 targetPos = new Vector2(character.target.transform.position.x, character.target.transform.position.z);

        return (targetPos - characterPos).magnitude < characterStats.AttackRadius;
    }
    protected virtual bool TargetOutOfDetectZone()
    {
        if (character.target == null)
        {
            return false;
        }
        Vector2 characterPos = new Vector2(character.transform.position.x, character.transform.position.z);
        Vector2 targetPos = new Vector2(character.target.transform.position.x, character.target.transform.position.z);

        return (targetPos - characterPos).magnitude > characterStats.DetectEnemyRadius;
    }
    public void SetTarget(Character Target)
    {
        character.target = Target;
    }
}