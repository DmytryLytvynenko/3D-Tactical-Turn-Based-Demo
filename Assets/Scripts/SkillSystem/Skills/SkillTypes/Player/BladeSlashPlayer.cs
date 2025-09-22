using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "BladeSlash", menuName = "ScriptableObjects/New Skill/New BladeSlash")]
public class BladeSlashPlayer : SkillBase
{
    private Tile selecedTile;
    private bool tileSelected;
    private List<Tile> skillTiles = new List<Tile>();
    [SerializeField] private GameObject VFX;
    public override void OnStart()
    {
        base.OnStart();
        Interact.TileSelected += OnTileSelected;
    }
    public override void OnEnd()
    {
        base.OnEnd();
        Interact.TileSelected -= OnTileSelected;
    }
    public override async Task UseSkill(CancellationToken ct, SkillParameters skillParameters = null)
    {
        OnSkillStarted();
        OnSkillSelected();
        Player.UsingSkill = true;
        tileSelected = false;
        SkillEnded = false;
        skillTiles = TileForms.NeighborTilesWithoutOrigin(Player.InstancePlayer.characterTile, _SkillData.Range);
        ShowSkillTiles();
        //skill active

        await TaskUtils.WaitUntil(() => tileSelected);
        if (ct.IsCancellationRequested)
        {
            return;
        }

        if (selecedTile == null) 
        {
            OnSkillCanceled();
            return;
        }
        if (!skillTiles.Contains(selecedTile))
        {
            OnSkillCanceled();
            return;
        }
        if (!selecedTile.Occupied)
        {
            OnSkillCanceled();
            return;
        }

        selecedTile.occupyingCharacter.gameObject.TryGetComponent(out HealthControll healthControll);
        if (healthControll == null)
        {
            Debug.LogWarning("Character has no health component");
            OnSkillCanceled();
            return;
        }

        OnSkillDeselected();
        ClearSkillTiles();

        await skillAgent.RotateToTarget(selecedTile.occupyingCharacter.transform.position);
        skillAgent.Character.animationController.SetTrigger(CharacterAnimParameters.BladeSlash);
        await Task.Delay(500);//time on animation etc=
/*        CameraShaker.Shake(ShakeType.Hit);
        CanvasController.Shake(ShakeType.CanvasHit);*/
        Instantiate(VFX, selecedTile.occupyingCharacter.characterCenter.position, Quaternion.identity);

        float damage = _SkillData.Damage;
        damage *= (skillAgent.transform.position.y - healthControll.transform.position.y) + 1;
        damage = (int)Mathf.Round(damage);
        healthControll.ChangeHealth(damage, Player.InstancePlayer);
        if (healthControll.Empty && !healthControll.Dead)
        {
            await selecedTile.occupyingCharacter.Die();
        }
        turnCounter = _SkillData.Cooldown;
        OnInteractableOn();

        if (OnCooldown)
        {
            OnInteractableOff();
        }
        SkillEnded = true;
        Player.UsingSkill = false;
        OnSkillEnded();
    }
    protected override void OnSkillCanceled()
    {
        base.OnSkillCanceled();
        ClearSkillTiles();
        turnCounter = 0;
        SkillEnded = true;
        Player.UsingSkill = false;
        OnInteractableOn();
    }
    private void OnTileSelected(Tile tile)
    {
        selecedTile = tile;
        tileSelected = true;
    }
    private void ShowSkillTiles()
    {
        if (skillTiles.Count == 0) return;

        foreach (Tile tile in skillTiles)
        {
            if (tile.Occupied)
            {
                tile.HighLight(Color.red);
            }
        }
    }
    private void ClearSkillTiles()
    {
        if (skillTiles.Count == 0) return;

        foreach (Tile tile in skillTiles)
        {
            if (tile.Occupied)
            {
                tile.ClearHighLight();
            }
        }
    }
    public override void ClearAllHightlight()
    {
       ClearSkillTiles();
    }
}
