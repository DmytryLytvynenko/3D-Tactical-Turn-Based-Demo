using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileForms : MonoBehaviour
{
    [SerializeField] private LayerMask tileMask;
    private static LayerMask s_tileMask;
    private void Awake()
    {
        s_tileMask = tileMask;
    }
    public static List<Tile> NeighborTiles(Tile origin, int radius)
    {
        if (radius == 0) return null;

        float rayLength = 10f;
        float rayHeightOffset = -2f;
        List<Tile> tiles = new List<Tile>();
        Tilemap tilemap = origin.tilemap;
        Vector3Int originPosition = tilemap.WorldToCell(origin.transform.position);
        int rows = 3 + (radius - 1) * 2;
        int minCells = radius + 1;
        int maxCells = 1 + radius * 2;
        List<Vector3Int> tilePositions = new List<Vector3Int>();
        List<Vector3Int> edgeCells = new List<Vector3Int>();

        //find edge cells
        if (originPosition.y % 2 == 0)
        {
            for (float i = originPosition.x - radius, j = originPosition.y, k = radius; k >= 0; i += 0.5f, j++, k--)
            {
                edgeCells.Add(new Vector3Int(Mathf.FloorToInt(i), Convert.ToInt32(j), originPosition.z));
            }
            for (float i = originPosition.x - radius, j = originPosition.y - 1, k = radius; k > 0; i += 0.5f, j--, k--)
            {
                edgeCells.Add(new Vector3Int(Mathf.CeilToInt(i), Convert.ToInt32(j), originPosition.z));
            }
        }
        else
        {
            for (float i = originPosition.x - radius + 0.5f, j = originPosition.y, k = radius; k >= 0; i += 0.5f, j++, k--)
            {
                edgeCells.Add(new Vector3Int(Mathf.FloorToInt(i), Convert.ToInt32(j), originPosition.z));
            }
            for (float i = originPosition.x - radius + 0.5f, j = originPosition.y - 1, k = radius; k > 0; i += 0.5f, j--, k--)
            {
                edgeCells.Add(new Vector3Int(Mathf.CeilToInt(i), Convert.ToInt32(j), originPosition.z));
            }
        }
        edgeCells = edgeCells.OrderBy(p => p.y).ToList();
        tilePositions.AddRange(edgeCells);

        //find neighbor tliles
        int cellInRow = minCells;
        int increment = 1;
        for (int i = 0; i < 1 + radius * 2; i++)
        {
            Vector3Int edgeCell = edgeCells[i];
            for (int j = 1; j < cellInRow; j++)
            {
                tilePositions.Add(new Vector3Int(edgeCell.x + j, edgeCell.y, 0));
            }
            if (cellInRow == maxCells)
                increment *= -1;
            cellInRow += increment;
        }

        for (int i = 0; i < tilePositions.Count; i++)
        {
            Vector3 tilePositionWorld = tilemap.CellToWorld(tilePositions[i]);
            Vector3 belowTilePos = new Vector3(tilePositionWorld.x, tilePositionWorld.y + rayHeightOffset, tilePositionWorld.z);
            if (Physics.Raycast(belowTilePos, Vector3.up, out RaycastHit hit, rayLength, s_tileMask))
            {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                tiles.Add(hitTile);
            }
        }
        return tiles;
    }
    public static List<Tile> NeighborTilesWithoutOrigin(Tile origin, int radius)
    {
        if (radius == 0) return null;

        float rayLength = 10f;
        float rayHeightOffset = -2f;
        List<Tile> tiles = new List<Tile>();
        Tilemap tilemap = origin.tilemap;
        Vector3Int originPosition = tilemap.WorldToCell(origin.transform.position);
        int rows = 3 + (radius - 1) * 2;
        int minCells = radius + 1;
        int maxCells = 1 + radius * 2;
        List<Vector3Int> tilePositions = new List<Vector3Int>();
        List<Vector3Int> edgeCells = new List<Vector3Int>();

        //find edge cells
        if (originPosition.y % 2 == 0)
        {
            for (float i = originPosition.x - radius, j = originPosition.y, k = radius; k >= 0; i += 0.5f, j++, k--)
            {
                edgeCells.Add(new Vector3Int(Mathf.FloorToInt(i), Convert.ToInt32(j), originPosition.z));
            }
            for (float i = originPosition.x - radius, j = originPosition.y - 1, k = radius; k > 0; i += 0.5f, j--, k--)
            {
                edgeCells.Add(new Vector3Int(Mathf.CeilToInt(i), Convert.ToInt32(j), originPosition.z));
            }
        }
        else
        {
            for (float i = originPosition.x - radius + 0.5f, j = originPosition.y, k = radius; k >= 0; i += 0.5f, j++, k--)
            {
                edgeCells.Add(new Vector3Int(Mathf.FloorToInt(i), Convert.ToInt32(j), originPosition.z));
            }
            for (float i = originPosition.x - radius + 0.5f, j = originPosition.y - 1, k = radius; k > 0; i += 0.5f, j--, k--)
            {
                edgeCells.Add(new Vector3Int(Mathf.CeilToInt(i), Convert.ToInt32(j), originPosition.z));
            }
        }
        edgeCells = edgeCells.OrderBy(p => p.y).ToList();
        tilePositions.AddRange(edgeCells);

        //find neighbor tliles
        int cellInRow = minCells;
        int increment = 1;
        for (int i = 0; i < 1 + radius * 2; i++)
        {
            Vector3Int edgeCell = edgeCells[i];
            for (int j = 1; j < cellInRow; j++)
            {
                tilePositions.Add(new Vector3Int(edgeCell.x + j, edgeCell.y, 0));
            }
            if (cellInRow == maxCells)
                increment *= -1;
            cellInRow += increment;
        }

        for (int i = 0; i < tilePositions.Count; i++)
        {
            Vector3 tilePositionWorld = tilemap.CellToWorld(tilePositions[i]);
            Vector3 aboveTilePos = new Vector3(tilePositionWorld.x, tilePositionWorld.y + rayHeightOffset, tilePositionWorld.z);
            if (Physics.Raycast(aboveTilePos, Vector3.up, out RaycastHit hit, rayLength, s_tileMask))
            {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                tiles.Add(hitTile);
            }
        }
        tiles.Remove(origin);
        return tiles;
    }
}
