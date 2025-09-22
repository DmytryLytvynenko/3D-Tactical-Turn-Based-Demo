using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SkillData", menuName = "ScriptableObjects/New SkillData")]
public class SkillData : ScriptableObject
{
    [field: SerializeField] public int   Range    { get; set; }
    [field: SerializeField] public int   Cooldown { get; set; }
    [field: SerializeField] public float Damage   { get; set; }
    [field: SerializeField] public int   Cost     { get; set; }

    [Header("Default")]
    [SerializeField] private int   defaultRange;
    [SerializeField] private int   defaultCooldown;
    [SerializeField] private float defaultDamage;
    [SerializeField] private int   defaultCost;

    public void ResetToDefault()
    {
        Range = defaultRange;
        Cooldown = defaultCooldown;
        Damage = defaultDamage;
        Cost = defaultCost;
    }
}
