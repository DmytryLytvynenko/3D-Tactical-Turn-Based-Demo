using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SheepTransform", menuName = "ScriptableObjects/New Skill/New SheepTransform")]
public class SheepTransform : SkillBase
{
    private Tile selecedTile;
    private bool tileSelected;
    private List<Tile> skillTiles = new List<Tile>();
    [SerializeField] private GameObject VFX;
    [SerializeField] private GameObject Sheep;
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

        Character character = selecedTile.occupyingCharacter;

        OnSkillDeselected();
        ClearSkillTiles();

        await skillAgent.RotateToTarget(selecedTile.occupyingCharacter.transform.position);
        skillAgent.Character.animationController.SetTrigger(CharacterAnimParameters.BladeSlash);
        await Task.Delay(500);//time on animation etc=
        /*        CameraShaker.Shake(ShakeType.Hit);
                CanvasController.Shake(ShakeType.CanvasHit);*/
        Instantiate(VFX, character.characterCenter.position, Quaternion.identity);


        character.characterTile.OnTileDeoccupiedByCharater();
        Vector3 spawnPos = new Vector3(selecedTile.transform.position.x, selecedTile.transform.position.y + 2, selecedTile.transform.position.z);
        Sheep sheep = Instantiate(Sheep, spawnPos, Quaternion.identity).GetComponent<Sheep>();
        sheep.characterMovement.FinalizePosition(selecedTile);
        ReferanceContainer.CharacterManager.RemoveCharacterFromActive(character);
        Destroy(character.gameObject);

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
