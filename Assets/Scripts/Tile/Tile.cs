using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Tile : MonoBehaviour
{
    #region member fields
    public Tile parent;
    public Tile connectedTile;
    public Character occupyingCharacter;
    public Tilemap tilemap;

    public float costFromOrigin = 0;
    public float costToDestination = 0;
    public int terrainCost = 0;
    public float TotalCost { get { return costFromOrigin + costToDestination + terrainCost; } }
    [field:SerializeField]public bool Occupied { get; set; } = false;

    private Outline outline;
    private HighlightTile highlight;
    #endregion

    #region events
    public event Action TileSelected;
    public event Action TileDeselected;
    public event Action<Character> TileOccupiedByCharater;    
    public event Action TileDeoccupiedByCharater;
    #endregion

    /// <summary>
    /// Changes color of the tile by activating child-objects of different colors
    /// </summary>
    /// <param name="col"></param>
    private void Awake()
    {
        outline = transform.GetComponent<Outline>();
        highlight = transform.GetComponent<HighlightTile>();
        tilemap = GetComponentInParent<Tilemap>();
        outline.enabled = false;
        outline.OutlineMode = global::Outline.Mode.OutlineAll;
        outline.OutlineWidth = 4;
    }

    public void Outline()
    {
        outline.enabled = true;
    }
    public void ClearOutline()
    {
        outline.enabled = false;
    }
    public void HighLight(UnityEngine.Color color)
    {
        highlight.HighLight(color);
    }       
    public void ClearHighLight()
    {
        highlight.ClearHighLight();
    }
    public void OnTileSelected() 
    {
        TileSelected?.Invoke();
        Outline();
    }
    public void OnTileDeselected()
    {
        TileDeselected?.Invoke();
        ClearOutline();
    }
    public void OnTileOccupiedByCharater(Character occupier)
    {
        Occupied = true;
        occupyingCharacter = occupier;
        TileOccupiedByCharater?.Invoke(occupier);
    }
    public void OnTileDeoccupiedByCharater()
    {
        Occupied = false;
        occupyingCharacter = null;
        TileDeoccupiedByCharater?.Invoke();
    }
}