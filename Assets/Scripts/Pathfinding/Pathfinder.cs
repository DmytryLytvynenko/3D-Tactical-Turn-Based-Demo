using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PathIllustrator))]
public class Pathfinder : MonoBehaviour
{
    #region member fields
    [SerializeField] private LayerMask tileMask;

    private static PathIllustrator illustrator;
    private static LayerMask s_tileMask;
    #endregion

    private void Start()
    {
        s_tileMask = tileMask;
        if (illustrator == null)
            illustrator = GetComponent<PathIllustrator>();
    }
    public static Path FindPath(Tile origin, Tile destination, int movePoints, bool showPath = false)
    {
        if (movePoints == 0) return null;
        List<Tile> openSet = new List<Tile>();
        List<Tile> closedSet = new List<Tile>();

        openSet.Add(origin);
        origin.costFromOrigin = 0;

        while (openSet.Count > 0)
        {
            openSet.Sort((x, y) => x.TotalCost.CompareTo(y.TotalCost));
            Tile currentTile = openSet[0];

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            //Destination reached
            if (currentTile == destination)
            {
                return PathBetween(destination, origin, movePoints, showPath);
            }

            foreach (Tile neighbor in TileForms.NeighborTiles(currentTile, 1))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                float costToNeighbor = currentTile.costFromOrigin + neighbor.terrainCost;
                if (Mathf.Abs(neighbor.transform.position.y - currentTile.transform.position.y) > 0.26f)
                {
                    costToNeighbor *= 10;
                }
                if (neighbor.Occupied)
                {
                    costToNeighbor *= 100;
                }
                if (costToNeighbor < neighbor.costFromOrigin || !openSet.Contains(neighbor))
                {
                    neighbor.costFromOrigin = costToNeighbor;
                    neighbor.costToDestination = Vector3.Distance(destination.transform.position, neighbor.transform.position);
                    neighbor.parent = currentTile;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }
    public static Path FindPath(Tile origin, Tile destination, bool showPath = false)
    {
        List<Tile> openSet = new List<Tile>();
        List<Tile> closedSet = new List<Tile>();

        openSet.Add(origin);
        origin.costFromOrigin = 0;

        while (openSet.Count > 0)
        {
            openSet.Sort((x, y) => x.TotalCost.CompareTo(y.TotalCost));
            Tile currentTile = openSet[0];

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            //Destination reached
            if (currentTile == destination)
            {
                return PathBetween(destination, origin, showPath);
            }

            foreach (Tile neighbor in TileForms.NeighborTiles(currentTile, 1))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                float costToNeighbor = currentTile.costFromOrigin + neighbor.terrainCost;
                if (Mathf.Abs(neighbor.transform.position.y - currentTile.transform.position.y) > 0.25f)
                {
                    costToNeighbor *= 10;
                }
                if (neighbor.Occupied)
                {
                    costToNeighbor *= 100;
                }
                if (costToNeighbor < neighbor.costFromOrigin || !openSet.Contains(neighbor))
                {
                    neighbor.costFromOrigin = costToNeighbor;
                    neighbor.costToDestination = Vector3.Distance(destination.transform.position, neighbor.transform.position);
                    neighbor.parent = currentTile;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }
    public static List<Tile> ReachableTiles(Tile origin, int movePoints)
    {
        List<Tile> tiles;
        CalculateCostFromOrigin(origin, movePoints, out tiles);
        return tiles;
    }
    public static Tile GetTileByPosition(Vector3 position, Tile origin)
    {
        Tile tile;
        int rayHeightOffset = 10;
        int rayLength = 100;
        Vector3 aboveTilePos = new Vector3(position.x, position.y + rayHeightOffset, position.z);
        if (Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit, rayLength, s_tileMask))
        {
            tile = hit.transform.GetComponent<Tile>();
            return tile;
        }
        return null;
    }
    private static void CalculateCostFromOrigin(Tile origin, int radius, out List<Tile> result)
    {
        float tileCount = 6 * (0.5f * radius * (radius + 1));
        List<Tile> openSet = new List<Tile>();
        List<Tile> closedSet = new List<Tile>();

        openSet.Add(origin);
        origin.costFromOrigin = 0;

        while (openSet.Count > 0 && closedSet.Count <= tileCount)
        {
            //openSet.Sort((x, y) => x.TotalCost.CompareTo(y.TotalCost));
            Tile currentTile = openSet[0];

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);
            
            foreach (Tile neighbor in TileForms.NeighborTiles(currentTile, 1))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                float costToNeighbor = currentTile.costFromOrigin + neighbor.terrainCost;
                if (Mathf.Abs(neighbor.transform.position.y - currentTile.transform.position.y) > 0.25f)
                {
                    costToNeighbor *= 10;
                }
                if (neighbor.Occupied)
                {
                    costToNeighbor *= 100;
                }
                if (costToNeighbor < neighbor.costFromOrigin || !openSet.Contains(neighbor))
                {
                    neighbor.costFromOrigin = costToNeighbor;
                    neighbor.parent = currentTile;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        result = closedSet;
        for (int i = 0; i < result.Count; i++)
        {
            if (result[i].costFromOrigin > radius)
            {
                result.RemoveAt(i);
                i--;
            }
        }
        for (int i = 0; i < result.Count; i++)
        {
            if (result[i].Occupied)
            {
                result.RemoveAt(i);
                i--;
            }
        }
    }
    private static Path PathBetween(Tile dest, Tile source, int movePoints, bool showPath)
    {
        Path path = MakePath(dest, source);
        for (int i = 1; i < path.tiles.Count; i++)
        {
            if (path.tiles[i].costFromOrigin - path.tiles[i - 1].costFromOrigin != 1)
            {
                Debug.Log("Height difference somewhere in path");
                return null;
            }
            movePoints -= path.tiles[i].terrainCost;
        }
        if (movePoints < 0)
        {
            Debug.Log("Not enough move points");
            return null;
        }
        if (showPath) illustrator.IllustratePath(path);
        return path;
    }
    private static Path PathBetween(Tile dest, Tile source, bool showPath)
    {
        Path path = MakePath(dest, source);
        if (showPath) illustrator.IllustratePath(path);
        return path;
    }
    private static Path MakePath(Tile destination, Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        Tile current = destination;

        while (current != origin)
        {
            tiles.Add(current);
            if (current.parent != null)
                current = current.parent;
            else
                break;
        }

        tiles.Add(origin);
        tiles.Reverse();

        Path path = new Path();
        path.tiles = tiles;

        return path;
    }
}
