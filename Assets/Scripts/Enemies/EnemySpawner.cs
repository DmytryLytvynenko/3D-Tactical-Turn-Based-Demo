using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<EnemySpawnChance> enemySpawnChances = new();
    [SerializeField] private GameObject SpawnVFX;
    [SerializeField] private CharacterManager characterManager;
    public int MobCap = 3;
    public int SpawnCooldown = 1;
    public int EnemiesPerSpawn = 1;
    public float spawnDistanceFromPlayer = 9;
    public float spawnDistanceFromPlayerDelta = .5f;
    public LayerMask tileMask;
    private int spawnCounter = 0;
    private void OnEnable()
    {
        LevelCounter.LevelIncreased += OnLevelIncreased;
        TurnSwitcher.TurnSwitched += OnTurnSwitched;
        CharacterManager.EnemiesTurnEnded += OnEnemiesTurnEnded;
    }
    private void OnDisable()
    {
        LevelCounter.LevelIncreased -= OnLevelIncreased;
        TurnSwitcher.TurnSwitched -= OnTurnSwitched;
        CharacterManager.EnemiesTurnEnded -= OnEnemiesTurnEnded;
    }
    public void SpawnEnemy()
    {
        GameObject _enemy = null;
        for (int i = 0; i < EnemiesPerSpawn; i++)
        {
            int random = UnityEngine.Random.Range(0,100);
            foreach (EnemySpawnChance enemy in enemySpawnChances)
            {
                if (random < enemy.SpawnChance)
                {
                    _enemy = enemy.Enemy;
                    break;
                }
            }
            Tile spawnTile = FindSpawnTile();
            Character character = Instantiate(_enemy, spawnTile.transform.position, Quaternion.identity).GetComponent<Character>();
            character.characterMovement.FinalizePosition(spawnTile);
            Instantiate(SpawnVFX, character.characterCenter.position, Quaternion.identity);
            //characterManager.AddCharacterToActive(character);
        }
    }
    public Tile FindSpawnTile()
    {
        Tile result = null;
        for (int i = 0; i < 50 || result == null; i++)
        {
            float x = UnityEngine.Random.Range(-Mathf.Sqrt(spawnDistanceFromPlayer) - spawnDistanceFromPlayerDelta, Mathf.Sqrt(spawnDistanceFromPlayer) + spawnDistanceFromPlayerDelta);
            float y = UnityEngine.Random.Range(Mathf.Sqrt(spawnDistanceFromPlayer - Mathf.Pow(x, 2)), Mathf.Sqrt(spawnDistanceFromPlayer) + spawnDistanceFromPlayerDelta);
            y = float.IsNaN(y) ? UnityEngine.Random.Range(-Mathf.Sqrt(spawnDistanceFromPlayer) - spawnDistanceFromPlayerDelta, Mathf.Sqrt(spawnDistanceFromPlayer) + spawnDistanceFromPlayerDelta) : y;
            y *= UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;

            Vector3 SpawnPoint = new Vector3(Player.InstancePlayer.transform.position.x + x,
                                             20,
                                             Player.InstancePlayer.transform.position.z + y);
            if (Physics.Raycast(SpawnPoint, -transform.up, out RaycastHit hit, 50f, tileMask))
            {
                Tile tile = hit.transform.GetComponent<Tile>();
                if (!tile.Occupied)
                {
                    Debug.DrawRay(tile.transform.position, Vector3.up * 100, Color.green, 100f);
                    result = tile;
                    break;
                }
            }
            //Debug.DrawRay(SpawnPoint, Vector3.down * 50f, Color.red, 5f);
        }
        return result;
    }
    public void FindSpawnTileX1000()
    {
        int i = 0;
        while(i <1001)
        {
            FindSpawnTile();
            i++;
        }
    }
    private void OnTurnSwitched()
    {
        if (characterManager.ActiveCharacterAmount < MobCap)
        {
            spawnCounter++;
        }
    }
    private void OnEnemiesTurnEnded()
    {
        if (spawnCounter >= SpawnCooldown)
        {
            if (characterManager.ActiveCharacterAmount < MobCap)
            {
                SpawnEnemy();
                spawnCounter -= SpawnCooldown;
            }    
        }
    }
    private void OnLevelIncreased()
    {
        MobCap = MobCap + (Mathf.Clamp(LevelCounter.LevelCount - 5,0,15)) / 2;
        if (LevelCounter.LevelCount == 7)
        {
            SpawnCooldown -= 1;
            int index = enemySpawnChances.FindIndex(x => x.EnemyType == EnemyType.Sheep);
            if (index != -1)
            {
                var temp = enemySpawnChances[index]; 
                temp.SpawnChance = 60;               
                enemySpawnChances[index] = temp;     
            }
            index = enemySpawnChances.FindIndex(x => x.EnemyType == EnemyType.Archer);
            if (index != -1)
            {
                var temp = enemySpawnChances[index]; 
                temp.SpawnChance = 95;               
                enemySpawnChances[index] = temp;     
            }
        }
        if (LevelCounter.LevelCount == 14)
        {
            SpawnCooldown -= 1;
            int index = enemySpawnChances.FindIndex(x => x.EnemyType == EnemyType.Sheep);
            if (index != -1)
            {
                var temp = enemySpawnChances[index];
                temp.SpawnChance = 40;
                enemySpawnChances[index] = temp;
            }
            index = enemySpawnChances.FindIndex(x => x.EnemyType == EnemyType.Archer);
            if (index != -1)
            {
                var temp = enemySpawnChances[index];
                temp.SpawnChance = 80;
                enemySpawnChances[index] = temp;
            }
        }
    }
}

[Serializable]
public struct EnemySpawnChance
{
    public EnemyType EnemyType;
    public float SpawnChance;
    public GameObject Enemy;
}
public enum EnemyType
{
    Sheep,
    Barbarian,
    Archer
}
