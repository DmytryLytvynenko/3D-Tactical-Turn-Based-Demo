using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ArcherBowShot", menuName = "ScriptableObjects/New Skill/New ArcherBowShot")]
public class ArcherBowShot : SkillBase
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float flyTime = 1;

    private bool projectileArrived = false;

    public override async Task UseSkill(CancellationToken ct, SkillParameters skillParameters = null)
    {
        float damage = 0;
        if (skillParameters == null)
        {
            Debug.LogWarning("ArcherBowShot - no target");
            return;
        }
        Character Target = skillParameters.Targets[0];
        Projectile projectile = Instantiate(arrowPrefab, skillParameters.shootPoint.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.ProjectileArrived += OnProjectileArrived;
        projectile.FlyTime = flyTime;
        projectile.Target = Target.characterCenter.position;

        SingleInstanceContainer.Camera.SetAnchor(projectile.transform);

        await TaskUtils.WaitUntil(() => projectileArrived);
        if (Target.TryGetComponent(out HealthControll healthControll))
        {
            damage = _SkillData.Damage;
            damage *= (skillAgent.transform.position.y - healthControll.transform.position.y) + 1;
            damage = (int)damage;
            healthControll.ChangeHealth(damage);
        }
        if (healthControll.Empty && !healthControll.Dead)
        {
            await Target.Die();
        }
        await Task.Delay(300);
        SingleInstanceContainer.Camera.SetAnchor(skillAgent.transform);
        projectile.ProjectileArrived -= OnProjectileArrived;
        projectileArrived = false;
        OnSkillEnded();
    }
    private void OnProjectileArrived()
    {
        projectileArrived = true;
    }
}
