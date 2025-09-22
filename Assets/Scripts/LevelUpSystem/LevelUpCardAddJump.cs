using UnityEngine;
using System;

[CreateAssetMenu(fileName = "AddJump", menuName = "LevelUPCardCollection/NewAddJump")]
public class LevelUpCardAddJump : LevelUpCard
{
    [SerializeField] private PlayerJump jump;
    private SkillAgent targetAgent;

    public static event Action JumpAdded;

    public override void Init()
    {
        targetAgent = Player.InstancePlayer.gameObject.GetComponent<SkillAgent>();
    }
    public override void ApplyCardEffect()
    {
        targetAgent.AddSkill(jump);
        JumpAdded?.Invoke();
    }
}
