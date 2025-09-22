using System;
using UnityEngine;

[Serializable]
public abstract class LevelUpCard : ScriptableObject
{
    [field: SerializeField] public Color CardColor { get; private set; } = Color.white;
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public LevelUpCardTypes CardType { get; protected set; }

    public virtual void ApplyCardEffect() { }
    public virtual void Init() { }
}
public enum LevelUpCardTypes
{
    Health1,
    Health2,
    Health3,
    ActionPoints,
    HealthRegen1,
    HealthRegen2,
    JumpDistanceUpgarde,
    JumpTimeUpgarde,
    JumpCostUpgarde,
    JumpAdd,
    KickAdd,
    AddUFO,
    KickTimeUpgarde,
    KickCostUpgarde,
}
