using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AddSheepTransform", menuName = "LevelUPCardCollection/NewSheepTransform")]
public class LevelUpCardAddSheepTransform : LevelUpCard
{
    [SerializeField] private SheepTransform sheepTransform;
    private SkillAgent targetAgent;

    public static event Action SheepTransformAdded;

    public override void Init()
    {
        targetAgent = Player.InstancePlayer.gameObject.GetComponent<SkillAgent>();
    }
    public override void ApplyCardEffect()
    {
        targetAgent.AddSkill(sheepTransform);
        SheepTransformAdded?.Invoke();
    }
}