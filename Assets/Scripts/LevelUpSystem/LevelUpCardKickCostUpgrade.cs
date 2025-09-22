using UnityEngine;

[CreateAssetMenu(fileName = "KickCostUpgrade", menuName = "LevelUPCardCollection/NewCardKickCostUpgrade")]
public class LevelUpCardKickCostUpgrade : LevelUpCard
{
    [SerializeField] private int kickCostDecrement;
    [SerializeField] private SkillData targetData;
    public override void ApplyCardEffect()
    {
        targetData.Cost += kickCostDecrement;
    }
}
