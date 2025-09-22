using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Kick", menuName = "ScriptableObjects/New Skill/New Kick")]
public class Kick : SkillBase
{
    [SerializeField] private LayerMask tileMask;
    [SerializeField] private GameObject VFX;
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
        if (!selecedTile.Occupied)
        {
            SkillCanceled();
            return;
        }

        selecedTile.occupyingCharacter.gameObject.TryGetComponent(out HealthControll healthControll);
        Character selectedCharacter = selecedTile.occupyingCharacter;
        if (healthControll == null)
        {
            Debug.LogWarning("Character has no health component");
            SkillCanceled();
            return;
        }

        OnSkillDeselected();
        OnInteractableOff();
        ClearSkillTiles();

        await skillAgent.RotateToTarget(selecedTile.occupyingCharacter.transform.position);
        skillAgent.Character.animationController.SetTrigger(CharacterAnimParameters.Kick);
        await Task.Delay(500);//time on animation etc
/*        CameraShaker.Shake(ShakeType.Hit);
        CanvasController.Shake(ShakeType.CanvasHit);*/
        Instantiate(VFX, selecedTile.occupyingCharacter.characterCenter.position, Quaternion.identity);

        float damage = _SkillData.Damage;
        damage *= (skillAgent.transform.position.y - healthControll.transform.position.y) + 1;
        damage = (int)Mathf.Round(damage);
        healthControll.ChangeHealth(damage, Player.InstancePlayer);
        if (healthControll.Empty)
        {
            selecedTile.occupyingCharacter.animationController.SetBool(CharacterAnimParameters.Dead, true);
        }
        Tile kickTile = NextTilePositionBehindCharacter(selecedTile, out Vector3 nextTilePos);
        if (kickTile == null)
        {
            //there is no tile(pit/abyss)
            //work with imaginary position
        }
        else
        {
            await selecedTile.occupyingCharacter.characterMovement.MoveToTile(kickTile, 0.4f);
            await Task.Delay(300);
        }
        if (healthControll.Empty && !healthControll.Dead)
        {
            await selectedCharacter.Die();
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

    protected AnimationCurve MakeCurve(Vector3 startPosition, Vector3 nextTilePosition)
    {
        AnimationCurve jumpCurve = new AnimationCurve();
        float tungent = 1.8f;
        float jumpOffset = 0.35f;
        float topKeyPosition;
        float firstKeyPosition = 0f;
        float lastKeyPosition = nextTilePosition.y - startPosition.y;
        if (startPosition.y > nextTilePosition.y)
            topKeyPosition = firstKeyPosition + jumpOffset;
        else
            topKeyPosition = lastKeyPosition + jumpOffset;

        jumpCurve.ClearKeys();
        jumpCurve.AddKey(0f, 0);
        jumpCurve.AddKey(0.5f, 0.5f);
        jumpCurve.AddKey(1f, 0);

        Keyframe keyframe = jumpCurve.keys[0];
        keyframe.outTangent = tungent;
        keyframe.value = firstKeyPosition;
        keyframe.time = 0f;
        jumpCurve.MoveKey(0, keyframe);

        keyframe = jumpCurve.keys[1];
        keyframe.outTangent = 0;
        keyframe.inTangent = 0;
        keyframe.value = topKeyPosition;
        keyframe.time = 0.5f;
        jumpCurve.MoveKey(1, keyframe);

        keyframe = jumpCurve.keys[2];
        keyframe.inTangent = -tungent;
        keyframe.value = lastKeyPosition;
        keyframe.time = 1f;
        jumpCurve.MoveKey(2, keyframe);

        return jumpCurve;
    }
    private Tile NextTilePositionBehindCharacter(Tile characterTile, out Vector3 outNextTilePosition)
    {
        Tile result = null;
        float rayHeightOffset = -100;
        float rayLength = Mathf.Abs(rayHeightOffset) + 10;
        Vector3 nextTileVector = new Vector3(characterTile.transform.position.x - skillAgent.transform.position.x,
                                             characterTile.transform.position.y,
                                             characterTile.transform.position.z - skillAgent.transform.position.z);
        Vector3 nextTilePosition = characterTile.transform.position + nextTileVector.normalized;
        outNextTilePosition = nextTilePosition;
        Vector3 belowTilePos = new Vector3(nextTilePosition.x, nextTilePosition.y + rayHeightOffset, nextTilePosition.z);
        if (Physics.Raycast(belowTilePos, Vector3.up, out RaycastHit hit, rayLength, tileMask))
        {
            Tile hitTile = hit.transform.GetComponent<Tile>();
            if (hitTile != null)
            {
                outNextTilePosition = hitTile.transform.position;
                result = hitTile;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
        if (result.Occupied)
        {
            return null;
        }
        if(Mathf.Abs(outNextTilePosition.y - characterTile.transform.position.y) > 0.26f)
        {
            return null;
        }
        return result;
    }
}
