using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "AxeSlashBarberian", menuName = "ScriptableObjects/New Skill/New AxeSlashBarberian")]
public class AxeSlashBarberian : SkillBase
{
    private List<Tile> skillTiles = new List<Tile>();
    [SerializeField] private GameObject VFX;

    public override async Task UseSkill(CancellationToken ct, SkillParameters skillParameters = null)
    {
        float damage = 0;
        if (skillParameters == null)
        {
            Debug.LogWarning("ArcherBowShot - skillParameters == null");
            return;
        }
        if (skillParameters.Targets == null)
        {
            Debug.LogWarning("ArcherBowShot - no target");
            return;
        }
        OnSkillStarted();
        OnSkillSelected();
        SkillEnded = false;
        Character Target = skillParameters.Targets[0];

        Instantiate(VFX, Target.characterCenter.position, Quaternion.identity);

        if (Target.TryGetComponent(out HealthControll healthControll))
        {
            damage = _SkillData.Damage;
            damage *= (skillAgent.transform.position.y - healthControll.transform.position.y) + 1;
            damage = (int)damage;
            healthControll.ChangeHealth(_SkillData.Damage);
        }
        else
        {
            Debug.LogWarning("Character has no health component");
            OnSkillCanceled();
            return;
        }

        if (healthControll.Empty && !healthControll.Dead)
        {
            await Target.Die();
        }
        SkillEnded = true;
        OnSkillEnded();
    }
}
