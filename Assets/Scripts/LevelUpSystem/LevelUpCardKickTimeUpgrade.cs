using UnityEngine;

[CreateAssetMenu(fileName = "KickTimeUpgrade", menuName = "LevelUPCardCollection/NewCardKickTimeUpgrade")]
public class LevelUpCardKickTimeUpgrade : LevelUpCard
{
    [SerializeField] private int kickTimeDecrement;
    [SerializeField] private SkillData targetData;
    public override void ApplyCardEffect()
    {
        targetData.Cooldown += kickTimeDecrement;
    }
}
