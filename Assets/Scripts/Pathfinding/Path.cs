using System.Collections.Generic;

[System.Serializable]
public class Path
{
    public List<Tile> tiles = new List<Tile>();
    public int Length { get { return tiles.Count; } }
    public Path(Tile tile1, Tile tile2)
    {
        tiles.Add(tile1);
        tiles.Add(tile2);
    }
    public Path()
    {

    }
    public void RemoveRange(int index, int count) 
    {
        tiles.RemoveRange(index, count); 
    }
    public Tile GetTile(int index)
    {
        return tiles[index];
    }
    public void RemoveAt(int index)
    {
        tiles.RemoveAt(index);
    }
}
