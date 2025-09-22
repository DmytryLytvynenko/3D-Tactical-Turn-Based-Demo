using UnityEngine;

[CreateAssetMenu(fileName = "NewActionPoints", menuName = "LevelUPCardCollection/NewActionPoints")]
public class LevelUPCardActionPoints : LevelUpCard
{
    [SerializeField] private int actionPointsIncreaseAmount;
    [SerializeField] private CharacterData targetData;
    public override void ApplyCardEffect()
    {
        targetData.ChangeParameter(CharacterDataParameter.ActionPoints, targetData.MaxActionPoints + actionPointsIncreaseAmount);
    }
}
