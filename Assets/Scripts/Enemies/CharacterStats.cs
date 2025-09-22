using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;
    public int ActionPoints { get; set; }
    public int ChaseTurns { get; set; }
    public float DetectEnemyRadius { get; private set; }
    public float AttackRadius { get; private set; }
    public int WalkingRadius { get; private set; }
    public LayerMask EnemyMask { get; private set; }

    public event Action StatsRestored;
    public event Action<int> ActionPointsUsed;

    private void Awake()
    {
        InitializeStats();
        RestoreStats();
        RestoreChaseTurns();
    }
    public void RestoreStats()
    {
        ActionPoints = characterData.MaxActionPoints;
        StatsRestored?.Invoke();
    }
    public void InitializeStats()
    {
        DetectEnemyRadius = characterData.DetectPlayerRadius;
        AttackRadius = characterData.AttackRadius;
        WalkingRadius = characterData.WalkingRadius;
        EnemyMask = characterData.EnemyMask;
    }
    public void RestoreChaseTurns()
    {
        ChaseTurns = characterData.ChaseTurns;
    }
    public void UseActionPoints(int usedPoints)
    {
        ActionPoints -= usedPoints;
        ActionPointsUsed?.Invoke(usedPoints);
    }
}
