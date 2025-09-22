using UnityEngine;

[CreateAssetMenu(fileName = "NewHealthCard", menuName = "LevelUPCardCollection/NewHealthCard")]
public class LevelUpCardHealth : LevelUpCard
{
    [SerializeField] private int healthIncreaseAmount;
    private HealthControll targetHealth;

    public override void Init()
    {
        targetHealth = Player.InstancePlayer.gameObject.GetComponent<HealthControll>();
    }
    public override void ApplyCardEffect()
    {
        targetHealth.ChangeMaxHealth(healthIncreaseAmount);
    }
}
