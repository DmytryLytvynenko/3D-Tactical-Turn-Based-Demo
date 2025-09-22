using UnityEngine;

[CreateAssetMenu(fileName = "CardJumpDistamceUpgrade", menuName = "LevelUPCardCollection/NewCardJumpDistamceUpgrade")]
public class LevelUpCardJumpDistanceUpgrade : LevelUpCard
{
    [SerializeField] private int jumpTileDistanceIncrement;
    [SerializeField] private SkillData targetData;
    public override void ApplyCardEffect()
    {
        targetData.Range += jumpTileDistanceIncrement;
    }
}
