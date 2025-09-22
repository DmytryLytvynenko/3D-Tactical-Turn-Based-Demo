using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Canvas localCanvas;
    [SerializeField] private Tile tile;

    private void Awake()
    {
        //localCanvas = GetComponent<Canvas>();
        //tile = GetComponent<Tile>();
    }
    private void OnEnable()
    {
        tile.TileSelected += OnTileSelected;
        tile.TileDeselected += OnTileDeselected;
        tile.TileOccupiedByCharater += OnTileOccupiedByCharater;
        tile.TileDeoccupiedByCharater += OnTileDeoccupiedByCharater;
    }
    private void OnDisable()
    {
        tile.TileSelected -= OnTileSelected;
        tile.TileDeselected -= OnTileDeselected;
        tile.TileOccupiedByCharater -= OnTileOccupiedByCharater;
        tile.TileDeoccupiedByCharater -= OnTileDeoccupiedByCharater;
    }
    private void ShowCanvas()
    {
        localCanvas.gameObject.SetActive(true);
    }
    private void HideCanvas()
    {
        localCanvas.gameObject.SetActive(false);
    }
    private void OnTileSelected()
    {
        //showinfo
    }
    private void OnTileDeselected()
    {
        //hideinfo
    }
    private void OnTileOccupiedByCharater(Character occupier)
    {
        if (occupier.IsPlayer)
            ShowCanvas();
    }
    private void OnTileDeoccupiedByCharater()
    {
        HideCanvas();
    }
}
