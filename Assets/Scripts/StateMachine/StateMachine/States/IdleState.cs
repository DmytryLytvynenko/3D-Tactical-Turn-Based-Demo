using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    private Tile anchorTile;
    public IdleState(Character character,
                     StateMachine stateMachine,
                     CharacterStats _characterStats,
                     Tile _anchorTile) : base(character, stateMachine, _characterStats)
    {
        anchorTile = _anchorTile;
    }
    protected virtual Tile RandomTileInRadius()
    {
        Tile result = null;
        if (AnchorTileIsFar())
        {
            Path pathToAnchor = Pathfinder.FindPath(character.characterTile, anchorTile);
            int tileIndex = Mathf.Clamp(0 + characterStats.ActionPoints,0, pathToAnchor.Length);
            result = pathToAnchor.tiles[tileIndex];
            return result;
        }
        else
        {
            List<Tile> neighbours = Pathfinder.ReachableTiles(character.characterTile, characterStats.ActionPoints);
            while (result == null)
            {
                int rand = UnityEngine.Random.Range(0, neighbours.Count);

                if (neighbours[rand].Occupied || neighbours[rand].terrainCost > characterStats.ActionPoints)
                    neighbours.RemoveAt(rand);
                else
                    result = neighbours[rand];
            }
            return result;
        }
    }
    protected virtual bool AnchorTileIsFar()
    {
        Vector3 anchorTileDist = anchorTile.transform.position - character.characterTile.transform.position;
        if (anchorTileDist.magnitude > characterStats.WalkingRadius)
            return true;
        else
            return false;
    }
    protected virtual bool TargetInDetectZone()
    {
        if (character.target == null) 
            return false; 

        Vector2 characterPos = new Vector2(character.transform.position.x, character.transform.position.z);
        Vector2 targetPos = new Vector2(character.target.transform.position.x, character.target.transform.position.z);

        return (targetPos - characterPos).magnitude < characterStats.DetectEnemyRadius;
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
}
