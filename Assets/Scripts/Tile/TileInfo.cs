using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum Biom
{
    Meadow,
    Desert,
    SnowMountains,
    River,
    None
}
[Serializable]
[CreateAssetMenu(fileName = "TileInfo", menuName = "ScriptableObjects/New TileInfo")]
public class TileInfo : ScriptableObject
{
    [field: SerializeField] public Biom Biom { get; private set; }
    [field: SerializeField, Range(0f,1f)] public float VisibilityFine { get; private set; } = 0;
    [field: SerializeField, Range(0f, 1f)] public float AttackRangeFine { get; private set; } = 0;
    [field: SerializeField, Range(0f, 2f)] public float SpeedMultiplier { get; private set; } = 1f;
    
}
