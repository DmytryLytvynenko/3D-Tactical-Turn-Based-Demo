using UnityEngine;

[CreateAssetMenu(fileName = "JumpCostUpgrade", menuName = "LevelUPCardCollection/NewCardJumpCostUpgrade")]
public class LevelUpCardJumpCostUpgrade : LevelUpCard
{
    [SerializeField] private int jumpTileCostDecrement;
    [SerializeField] private SkillData targetData;
    public override void ApplyCardEffect()
    {
        targetData.Cost += jumpTileCostDecrement;
    }
}
