using UnityEngine;

[CreateAssetMenu(fileName = "NewAddUFO", menuName = "LevelUPCardCollection/NewAddUFO")]
public class LevelUpCardAddUFO : LevelUpCard
{
    public override void ApplyCardEffect()
    {
        ReferanceContainer.UFOController.enabled = true;
        ReferanceContainer.UFOController.EnableUFO();
    }
}
