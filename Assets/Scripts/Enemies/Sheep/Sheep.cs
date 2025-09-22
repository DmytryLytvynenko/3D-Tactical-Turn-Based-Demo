using CartoonFX;
using SmoothShakeFree;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Sheep : Character
{
    #region member fields

    private Tile anchorTile;
    [SerializeField] private int explosionDamage;
    [SerializeField] private GameObject explosionVFX;

    #endregion

    protected override void Awake()
    {
        base.Awake();

    }
    protected override void Start()
    {
        base.Start();

        ReferanceContainer.CharacterManager.AddCharacterToActive(this);
        anchorTile = characterTile;

        idleState = new SheepIdleState(this, stateMachine, characterStats, anchorTile);
        battleState = new SheepBattleState(this, stateMachine, characterStats);
        switch (startState)
        {
            case StateType.BattleState:
                stateMachine.Initiallize(battleState);
                break;
            case StateType.IdleState:
                stateMachine.Initiallize(idleState);
                break;
            case StateType.ChaseState:
                break;
            default:
                break;
        }
        bool needToChangeState = false;
        stateMachine.CurrentState.FindTarget();
        stateMachine.CurrentState.OnStep(ref needToChangeState);
    }
    public override async Task Die()
    {
        GetComponent<HealthControll>().Dead = true;
        List<Tile> skillTiles = TileForms.NeighborTilesWithoutOrigin(characterTile, 1);
        animationController.SetBool(CharacterAnimParameters.Dead, true); //anim
        await Task.Delay(666);
        CFXR_Effect expl = Instantiate(explosionVFX, characterCenter.position, Quaternion.identity).GetComponent<CFXR_Effect>(); //visual effect
        InvokeCharacterDied();
        CameraShaker.Shake(ShakeType.StrongHit);
        CanvasController.Shake(ShakeType.StrongCanvasHit);
        HideModel();
        m_collider.enabled = false;
        foreach (Tile tile in skillTiles)
        {
            if (tile.Occupied)
            {
                HealthControll healthControll = tile.occupyingCharacter.GetComponent<HealthControll>();
                healthControll.ChangeHealth(explosionDamage);
                if (healthControll.Empty && !healthControll.Dead)
                {
                    await tile.occupyingCharacter.Die();
                }
            }
        }
        characterTile.OnTileDeoccupiedByCharater();
    }
}
