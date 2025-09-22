using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider),typeof(HealthControll))]
public class Character : MonoBehaviour
{
    #region member fields
    public bool Moving { get; set; } = false;
    public bool IsPlayer { get; protected set; } = false;
    public bool FinishedTurn { get; set; } = true;
    public bool HasPoints { get { return characterStats.ActionPoints > 0; } }
    public bool IsDead { get { return m_healthControll.Dead; } }

    public Tile characterTile;
    public CharacterStats characterStats;
    public CharacterMovement characterMovement;
    public AnimationController animationController;
    public TagsTargets tagsTargets;
    public Transform characterVisual;
    public Transform characterCenter;
    public Transform characterBottom;
    public Transform StatsCanvas;
    public event Action ActionMade;
    public event Action StatsRestored;
    public event Action<Character> CharacterDied;

    [SerializeField] protected StateType startState;
    [SerializeField] protected GameObject dieVFX;

    [Header("Debug")]
    [SerializeField] protected bool showCurrentState;
    [SerializeField] protected bool drawDistance;
    [SerializeField] protected Transform drawDistanceTarget;

    protected Outline outline;
    protected Collider m_collider;
    protected HealthControll m_healthControll;
    protected List<Tile> reachable; 
    protected Player player;
    protected Task CurrentStateTask;
    public Character target;
    protected CancellationTokenSource makeTurnTokenSource;
    #endregion

    #region StateMachineVariables
    protected StateMachine stateMachine { get; set; }
    public IdleState idleState; 
    public BattleState battleState; 
    public ChaseState chaseState; 
    #endregion

    protected virtual void Awake()
    {
        m_healthControll = GetComponent<HealthControll>();
        m_collider = GetComponent<Collider>();
        player = FindFirstObjectByType<Player>().GetComponent<Player>();
        characterMovement.FindTileAtStart();
        stateMachine = new StateMachine();
    }
    protected virtual void OnEnable()
    {
        TurnSwitcher.TurnSwitched += OnTurnSwitched;
        PlayerMovement.MadeStep += OnPlayerMadeStep;
    }
    protected virtual void OnDisable()
    {
        TurnSwitcher.TurnSwitched -= OnTurnSwitched;
        PlayerMovement.MadeStep -= OnPlayerMadeStep;
    }
    protected virtual void Start()
    {
        //characterMovement.FindTileAtStart();

        outline = GetComponent<Outline>();
        outline.enabled = false;
        characterVisual = transform.GetChild(0);
    }
    public void HighLight()
    {
        outline.enabled = true;
    }    
    public void ClearHighLight()
    {
        outline.enabled = false;
    }
    public void HideModel()
    {
        StatsCanvas.gameObject.SetActive(false);
        characterVisual.gameObject.SetActive(false);
    }    
    public void ShowModel()
    {
        StatsCanvas.gameObject.SetActive(true);
        characterVisual.gameObject.SetActive(true);
    }
    public virtual async void MakeTurn()
    {
        while (!FinishedTurn)
        {
            makeTurnTokenSource = new CancellationTokenSource();
            try
            {
                Task CurrentStateTask = stateMachine.CurrentState.MakeTurn(makeTurnTokenSource.Token);
                await CurrentStateTask;
            }
            catch (OperationCanceledException ex)
            {
                print("CurrentStateTask canceled");
                print(ex);
            }
            finally
            {
                makeTurnTokenSource.Dispose();
            }
        }
    }
    public void OnCharacterSelected() 
    {
        HighLight();
        reachable = Pathfinder.ReachableTiles(characterTile, characterStats.ActionPoints);
        if (reachable == null) return;

        foreach (var tile in reachable)
        {
            tile.Outline();
        }
    }
    public void OnCharacterDeselected()
    {
        ClearHighLight();
        if (reachable == null) return;

        foreach (var tile in reachable)
        {
            tile.ClearOutline();
        }
        reachable.Clear();
    }
    protected virtual void OnTurnSwitched()
    {
        characterStats.RestoreStats();
        StatsRestored?.Invoke();
        FinishedTurn = false;
    }
    protected virtual void OnPlayerMadeStep()
    {
        //check if need to switch state
        stateMachine.CurrentState.OnPlayerMadeStep();
    }
    public virtual void OnStep(ref bool needToChangeState) 
    {
        stateMachine.CurrentState.OnStep(ref needToChangeState);
    }

    public virtual async Task Die()
    {
        characterTile.OnTileDeoccupiedByCharater();
        GetComponent<HealthControll>().Dead = true;
        animationController.SetBool(CharacterAnimParameters.Dead, true);
        await Task.Delay(4000);
        Instantiate(dieVFX, characterCenter.transform.position, Quaternion.identity);
        HideModel();
        InvokeCharacterDied();
    }

    protected virtual void OnDrawGizmos()
    {
        if (showCurrentState && stateMachine != null)
        {
            GUIStyle debugTextStyle = new GUIStyle();
            debugTextStyle.fontSize = 16;
            debugTextStyle.normal.textColor = Color.black;
            string state = stateMachine.CurrentState.ToString();
            Vector3 pos = transform.position;
            pos.y += 0.5f;
            Handles.Label(pos, state, debugTextStyle);
        }
        if (drawDistance && drawDistanceTarget != null)
        {
            GUIStyle debugTextStyle = new GUIStyle();
            debugTextStyle.fontSize = 16;
            debugTextStyle.normal.textColor = Color.black;
            string distance1 = (transform.position - drawDistanceTarget.position).magnitude.ToSafeString();
            Vector3 pos = (transform.position + drawDistanceTarget.position) / 2;
            pos.y += 0.2f;
            Gizmos.DrawLine(transform.position, drawDistanceTarget.position);
            Handles.Label(pos, distance1, debugTextStyle);
        }
    }
    public void CancelCurentStateTask()
    {
        makeTurnTokenSource.Cancel();
    }
    public void InvokeActionMade()
    {
        ActionMade?.Invoke();
    }
    protected void InvokeCharacterDied()
    {
        CharacterDied?.Invoke(this);
    }
    protected void DestroyCharacter()
    {
        Destroy(gameObject);
    }
}