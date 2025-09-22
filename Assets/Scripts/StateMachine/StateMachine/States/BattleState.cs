using UnityEngine;

public class BattleState : State
{
    public BattleState(Character character,
                      StateMachine stateMachine,
                      CharacterStats _characterStats) : base(character, stateMachine, _characterStats) { }
    protected virtual bool TargetOutOfDetectZone()
    {
        if (character.target == null)
        {
            return false;
        }
        Vector2 characterPos = new Vector2(character.transform.position.x, character.transform.position.z);
        Vector2 tragetPos = new Vector2(character.target.transform.position.x, character.target.transform.position.z);

        return (tragetPos - characterPos).magnitude > characterStats.DetectEnemyRadius;
    }
    protected virtual bool TargetInAttackZone()
    {
        if (character.target == null)
        {
            return false;
        }
        Vector2 characterPos = new Vector2(character.transform.position.x, character.transform.position.z);
        Vector2 tragetPos = new Vector2(character.target.transform.position.x, character.target.transform.position.z);

        return (tragetPos - characterPos).magnitude < characterStats.AttackRadius;
    }
    protected virtual bool TargetOutOfAttackZone()
    {
        if (character.target == null)
        {
            return false;
        }
        Vector2 characterPos = new Vector2(character.transform.position.x, character.transform.position.z);
        Vector2 tragetPos = new Vector2(character.target.transform.position.x, character.target.transform.position.z);

        return (tragetPos - characterPos).magnitude > characterStats.AttackRadius;
    }
}
