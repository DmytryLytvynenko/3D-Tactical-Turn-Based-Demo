using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Debug = UnityEngine.Debug;

[RequireComponent(typeof(AudioSource))]
public class Interact : MonoBehaviour
{
    #region member fields
    [SerializeField] GraphicRaycaster m_Raycaster;
    [SerializeField] AudioClip click, pop;
    [SerializeField] LayerMask interactMask;
    [SerializeField] LayerMask showInfoMask;
    [SerializeField] LayerMask ignoreLayers;

    //Debug purposes only
    [SerializeField] bool debug;
    private Path Lastpath;

    private Camera mainCam;
    private Tile currentTile;
    private Character selectedCharacter;
    private InputActions inputActions;
    private IInfoShowable currentInfo = null;
    #endregion

    #region Events
    public static event Action<Tile> TileSelected;
    #endregion

    private void Awake()
    {
        print(SystemInfo.deviceType);
        inputActions = new InputActions();
        mainCam = gameObject.GetComponent<Camera>();
    }
    private void OnEnable()
    {
        SkillUIButton.SkillButtonPressed += OnSkillButtonPressed;

        inputActions.Player.MousePosition.Enable();
        inputActions.Player.Interact.Enable();
        inputActions.Player.RMBInfo.Enable();
        inputActions.Player.Interact.performed += _ =>
        {
            ScreenTap(inputActions.Player.MousePosition.ReadValue<Vector2>());
        };

        inputActions.Player.RMBInfo.performed += _ =>
        {
            ShowInfo(inputActions.Player.MousePosition.ReadValue<Vector2>());
        };
    }
    private void OnDisable()
    {
        SkillUIButton.SkillButtonPressed -= OnSkillButtonPressed;
        inputActions.Player.MousePosition.Disable();
        inputActions.Player.Interact.Disable();
        inputActions.Player.RMBInfo.Disable();
    }
    private void ShowInfo(Vector2 tapPos)
    {
        EventSystem eventSystem = EventSystem.current;
        PointerEventData m_PointerEventData = new PointerEventData(eventSystem);
        m_PointerEventData.position = tapPos;

        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);
        if (results.Count > 0)
        {
            if (currentInfo != null)
            {
                currentInfo.HideInfo();
                currentInfo = null;
            }
        }

        Debug.DrawLine(transform.position, mainCam.ScreenPointToRay(tapPos).GetPoint(200f), Color.red, 10f);
        if (Physics.Raycast(mainCam.ScreenPointToRay(tapPos), out RaycastHit hit, 200f, showInfoMask))
        {
            if (hit.transform.TryGetComponent(out IInfoShowable infoShowable))
            {
                if (currentInfo == infoShowable)
                {
                    currentInfo.HideInfo();
                    currentInfo = null;
                    return;
                }
                if (currentInfo != null)
                {
                    currentInfo.HideInfo(); 
                }
                currentInfo = infoShowable;
                currentInfo.ShowInfo();
                return;
            }
        }
        if (currentInfo != null)
        {
            currentInfo.HideInfo();
            currentInfo = null;
        }
    }
    private void ScreenTap(Vector2 tapPos)
    {
        if (currentInfo != null)
        {
            currentInfo.HideInfo();
            currentInfo = null;
        }

        EventSystem eventSystem = EventSystem.current;
        PointerEventData m_PointerEventData = new PointerEventData(eventSystem);
        m_PointerEventData.position = tapPos;

        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);
        if (results.Count > 0)
        {
            //Debug.Log("Hit " + LayerMask.LayerToName(results[0].gameObject.layer));
            if (ignoreLayers.Contains(results[0].gameObject.layer))
            {
                print("Cursor not on Tile");
                return;
            }
        }


        Debug.DrawLine(transform.position, mainCam.ScreenPointToRay(tapPos).GetPoint(200f), Color.red, 10f);
        if (!Physics.Raycast(mainCam.ScreenPointToRay(tapPos), out RaycastHit hit, 200f, interactMask))
        {
            Clear();
            selectedCharacter = null;
            return;
        }
        if (ignoreLayers.Contains(hit.transform.gameObject.layer))
        {
            print("Cursor not on Tile");
            return;
        }
        if (currentTile != hit.transform.GetComponent<Tile>())
        {
            Clear();
        }
        currentTile = hit.transform.GetComponent<Tile>();
        InspectTile();
    }
    private void InspectTile()
    {
        TileSelected?.Invoke(currentTile);
        if (Player.UsingSkill) return;

        currentTile.OnTileSelected();
        if (currentTile.Occupied)
        {
            if (selectedCharacter == currentTile.occupyingCharacter)
            {
                selectedCharacter.OnCharacterDeselected();
                selectedCharacter = null;
                return;
            }
            InspectCharacter();
        }
        else
        {
            NavigateToTile();
        }
    }
    private void InspectCharacter()
    {
        if (currentTile.occupyingCharacter.Moving)
            return;

        SelectCharacter();
    }
    private void Clear()
    {
        if (currentTile == null)
            return;

        //currentTile.ClearOutline();
        currentTile.OnTileDeselected();
        if (selectedCharacter)
        {
            selectedCharacter.OnCharacterDeselected();
        }
    }
    private void SelectCharacter()
    {
        selectedCharacter = currentTile.occupyingCharacter;
        selectedCharacter.OnCharacterSelected();

        //GetComponent<AudioSource>().PlayOneShot(pop);
    }
    private void NavigateToTile()
    {
        if (selectedCharacter == null || selectedCharacter.Moving == true)
            return;
        if (!selectedCharacter.IsPlayer)
        {
            selectedCharacter.OnCharacterDeselected();
            selectedCharacter = null;
            return;
        }

        if (RetrievePath(out Path newPath))
        {
            selectedCharacter.characterMovement.StartMove(newPath);
        }
        selectedCharacter.OnCharacterDeselected(); 
        selectedCharacter = null;
    }

    private bool RetrievePath(out Path path)
    {
        path = Pathfinder.FindPath(selectedCharacter.characterTile, currentTile, selectedCharacter.characterStats.ActionPoints, true);
        if (path == null || path == Lastpath)
            return false;

        return true;
    }
    private void OnSkillButtonPressed()
    {
        Clear();
        selectedCharacter = null;
        return;
    }
}