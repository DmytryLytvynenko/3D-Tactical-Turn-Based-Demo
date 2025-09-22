using UnityEngine;

[CreateAssetMenu(fileName = "CardJumpTimeUpgrade", menuName = "LevelUPCardCollection/NewCardJumpTimeUpgrade")]
public class LevelUpCardJumpTimeUpgrade : LevelUpCard
{
    [SerializeField] private int jumpTileTimeDecrement;
    [SerializeField] private SkillData targetData;
    public override void ApplyCardEffect()
    {
        targetData.Cooldown += jumpTileTimeDecrement;
    }
}
