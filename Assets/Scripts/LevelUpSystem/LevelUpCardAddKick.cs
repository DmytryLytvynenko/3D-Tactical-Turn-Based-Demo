using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AddKick", menuName = "LevelUPCardCollection/NewAddKick")]
public class LevelUpCardAddKick : LevelUpCard
{
    [SerializeField] private Kick kick;
    private SkillAgent targetAgent;

    public static event Action KickAdded;

    public override void Init()
    {
        targetAgent = Player.InstancePlayer.gameObject.GetComponent<SkillAgent>();
    }
    public override void ApplyCardEffect()
    {
        targetAgent.AddSkill(kick);
        KickAdded?.Invoke();
    }
}
