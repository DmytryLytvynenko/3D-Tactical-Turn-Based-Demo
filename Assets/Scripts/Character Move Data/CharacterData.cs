using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/new Character Data", order = 1)]
public class CharacterData : ScriptableObject
{
    [field: SerializeField] public int MaxActionPoints { get; private set; }
    [field: SerializeField] public float DetectPlayerRadius { get; private set; }
    [field: SerializeField] public float AttackRadius { get; private set; }
    [field: SerializeField] public int WalkingRadius { get; private set; }
    [field: SerializeField] public int ChaseTurns { get; private set; }
    [field: SerializeField] public int ExpiriencePoints { get; private set; }
    [field: SerializeField] public LayerMask EnemyMask { get; private set; }
    
    [Header("Default")]
    [SerializeField] private int   defaultMaxActionPoints;
    [SerializeField] private float defaultDetectPlayerRadius;
    [SerializeField] private float defaultAttackRadius;
    [SerializeField] private int   defaultWalkingRadius;
    [SerializeField] private int   defaultChaseTurns;
    [SerializeField] private int   defaultExpiriencePoints;
    [SerializeField] private LayerMask defaultEnemyMask;

    public event Action<int> MaxActionPointsChanged;

    private void OnDisable()
    {
        ResetToDefault();
    }

    public void ResetToDefault()
    {
        MaxActionPoints = defaultMaxActionPoints;
        DetectPlayerRadius = defaultDetectPlayerRadius;
        AttackRadius = defaultAttackRadius;
        WalkingRadius = defaultWalkingRadius;
        ChaseTurns = defaultChaseTurns;
        ExpiriencePoints = defaultExpiriencePoints;
        EnemyMask = defaultEnemyMask;
    }

    public void ChangeParameter(CharacterDataParameter parameter, float value)
    {
        switch (parameter)
        {
            case CharacterDataParameter.ActionPoints:
                MaxActionPoints = (int)value;
                MaxActionPointsChanged(MaxActionPoints);
                break;
            case CharacterDataParameter.AttackRadius:
                AttackRadius = (int)value;
                break;
            case CharacterDataParameter.DetectPlayerRadius: 
                DetectPlayerRadius = (int)value;
                break;
            case CharacterDataParameter.WalkingRadius:
                WalkingRadius = (int)value;
                break;
            case CharacterDataParameter.ChaseTurns:
                ChaseTurns = (int)value;
                break;
            default: 
                break;
        }
    }
}
public enum CharacterDataParameter 
{
    ActionPoints,
    DetectPlayerRadius,
    AttackRadius,
    WalkingRadius,
    ChaseTurns
}
