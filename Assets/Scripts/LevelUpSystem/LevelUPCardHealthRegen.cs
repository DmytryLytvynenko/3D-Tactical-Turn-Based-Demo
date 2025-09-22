using UnityEngine;

[CreateAssetMenu(fileName = "HealthRegenCard", menuName = "LevelUPCardCollection/NewHealthRegenCard")]
public class LevelUPCardHealthRegen : LevelUpCard
{
    [SerializeField] private int healthRegenAmount;
    private HealthControll targetHealth;

    public override void Init()
    {
        targetHealth = Player.InstancePlayer.gameObject.GetComponent<HealthControll>();
    }
    public override void ApplyCardEffect()
    {
        targetHealth.RegenPoints = healthRegenAmount;
    }
}
