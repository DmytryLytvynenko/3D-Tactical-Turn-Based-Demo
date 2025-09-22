using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum StateType
{
    BattleState,
    IdleState,
    ChaseState
}
public abstract class State 
{
    protected Character character;
    protected CharacterStats characterStats;
    protected StateMachine stateMachine;
    public State(Character character, StateMachine stateMachine, CharacterStats characterStats)
    {
        this.character = character;
        this.stateMachine = stateMachine;
        this.characterStats = characterStats;
        stateMachine.AddState(this);
    }
    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void OnPlayerMadeStep() 
    {
        Debug.Log("OnPlayerMadeStep");
        FindTarget();
    }
    public virtual void OnStep(ref bool needToChangeState) 
    {
        FindTarget();
    }
    public virtual Task MakeTurn(CancellationToken ct)
    {
        return Task.CompletedTask;
    }
    public virtual void FindTarget()
    {
        if (character.target != null)
        {
            return;
        }
        List<Character> characters = new List<Character>();
        List<Tile> tiles = TileForms.NeighborTilesWithoutOrigin(character.characterTile, (int)characterStats.DetectEnemyRadius);
        if (tiles.Count == 0)
        {
            character.target = null;
            return;
        }
        foreach (Tile tile in tiles) 
        {
            if (tile.Occupied)
            {
                if (character.tagsTargets.TargetsContains(tile.occupyingCharacter.tagsTargets.Tags))
                {
                    characters.Add(tile.occupyingCharacter);
                }
            }
        }

        characters = character.tagsTargets.SortByTargetPriorityAndDistance(character.transform.position ,characters);
        if (characters.Count == 0)
        {
            return;
        }
        character.target = characters.First();
    }
    public virtual List<Character> SearchForEnemies()
    {
        List<Character> characters = new List<Character>();
        List<Tile> tiles = TileForms.NeighborTilesWithoutOrigin(character.characterTile, (int)characterStats.DetectEnemyRadius);
        foreach (Tile tile in tiles)
        {
            if (tile.Occupied)
            {
                characters.Add(tile.occupyingCharacter);
            }
        }
        //
        // make filters
        //

        characters.OrderBy(x => (x.transform.position - character.transform.position).magnitude);
        return characters;
    }
}
