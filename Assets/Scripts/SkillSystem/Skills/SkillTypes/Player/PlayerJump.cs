using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "PlayerJump", menuName = "ScriptableObjects/New Skill/PlayerJump")]
public class PlayerJump : SkillBase
{
    private Tile selecedTile;
    private bool tileSelected;
    private List<Tile> skillTiles = new List<Tile>();
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
            SkillCanceled();
            return;
        }
        if (!skillTiles.Contains(selecedTile))
        {
            SkillCanceled();
            return;
        }
        if (selecedTile.Occupied)
        {
            SkillCanceled();
            return;
        }

        ClearSkillTiles();
        await skillAgent.Character.characterMovement.Teleport(selecedTile);

        OnSkillDeselected();
        await Task.Delay(500);//time on animation etc
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
    private void SkillCanceled()
    {
        OnSkillCanceled();
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
            if (!tile.Occupied)
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
            tile.ClearHighLight();
        }
    }
    public override void ClearAllHightlight()
    {
        ClearSkillTiles();
    }
}
