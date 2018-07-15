using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TileManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private int playerStartX;
    [SerializeField] private int playerStartY;
    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField] private Transform tileParent;
    [SerializeField] private Tile[,] tiles;
    [SerializeField] private int columnCount;
    [SerializeField]
    private int prevColumnCount;
    [SerializeField] private int rowCount;
    [SerializeField] private int prevRowCount;
    [SerializeField] private float spaceBetweenTiles = 1.5f;
    private bool isGame;

    private void Update()
    {
        if (columnCount == prevColumnCount && rowCount == prevRowCount)
        {
            return;
        }

        prevColumnCount = columnCount;
        prevRowCount = rowCount;

        if (tileParent == null)
        {
            return;
        }
        DestroyImmediate(tileParent.gameObject);
        tileParent = (new GameObject("TileParent")).transform;
        for (int i = 0; i < columnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                var newTile = Instantiate(tilePrefab, tileParent);
                newTile.transform.position = new Vector3(i * spaceBetweenTiles, 0, j * spaceBetweenTiles);
                newTile.name = i + "," + j;
            }
        }
    }

    private void Awake()
    {
        tiles = new Tile[columnCount, rowCount];
        foreach (Transform child in tileParent)
        {
            string[] split = child.name.Split(',');
            int x = int.Parse(split[0]);
            int y = int.Parse(split[1]);
            var tile = child.GetComponent<Tile>();
            tiles[x, y] = tile;
            tile.Init(this);
            tile.X = x;
            tile.Y = y;
        }

        player.CurrentTile = tiles[playerStartX, playerStartY];
        player.Init();
    }

    public void OnTileClick(Tile clickedTile)
    {
        var totalPath = AStar(player.CurrentTile, clickedTile);

        foreach (var tile in tiles)
        {
            tile.SetHighlight(false);
        }

        foreach (var tile in totalPath)
        {
            tile.SetHighlight(true);
        }

        player.Path = totalPath;
    }

    public int Distance(Tile t1, Tile t2)
    {
        int result = Mathf.Abs(t1.X - t2.X) + Mathf.Abs(t1.Y - t2.Y);
        return result;
    }

    /// <summary>
    /// https://en.wikipedia.org/wiki/A*_search_algorithm
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    private List<Tile> AStar(Tile start, Tile goal)
    {
        // Reset scores
        foreach (var tile in tiles)
        {
            tile.GScore = int.MaxValue;
            tile.FScore = int.MaxValue;
        }

        // The set of nodes already evaluated
        var closedSet = new HashSet<Tile>();

        // The set of currently discovered nodes that are not evaluated yet.
        // Initially, only the start node is known.
        var openSet = new HashSet<Tile>
        {
            start
        };

        // For each node, which node it can most efficiently be reached from.
        // If a node can be reached from many nodes, cameFrom will eventually contain the
        // most efficient previous step.
        var cameFrom = new Dictionary<Tile, Tile>();

        // The cost of going from start to start is zero.
        start.GScore = 0;

        // For the first node, that value is completely heuristic.
        start.FScore = Distance(start, goal);

        while (openSet.Count > 0)
        {
            Tile current = null;
            foreach (var tile in openSet)
            {
                if (current == null)
                {
                    current = tile;
                    continue;
                }
                if (tile.FScore < current.FScore)
                {
                    current = tile;
                }
            }

            if (current == goal)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            var neighbors = GetNeighbors(current);
            foreach (var neighbor in neighbors)
            {
                if (closedSet.Contains(neighbor))
                {
                    continue; // Ignore the neighbor which is already evaluated.
                }

                // The distance from start to a neighbor
                var tentativeGScore = current.GScore + GetNeighborDistance(current, neighbor);

                if (!openSet.Contains(neighbor)) // Discover a new node
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGScore >= neighbor.GScore)
                {
                    continue; // This is not a better path.
                }

                cameFrom[neighbor] = current;
                neighbor.GScore = tentativeGScore;
                neighbor.FScore = neighbor.GScore + Distance(neighbor, goal);
            }
        }

        Debug.Log("SOMETHING WENT WRONG...");
        return null;
    }

    private int GetNeighborDistance(Tile current, Tile neighbor)
    {
        if (current.isObstacle || neighbor.isObstacle)
        {
            return 9999;
        }
        return 1;
    }

    private int GetFScore(Tile start, Tile goal)
    {
        return Distance(start, goal);
    }

    private List<Tile> ReconstructPath(Dictionary<Tile, Tile> cameFrom, Tile current)
    {
        var totalPath = new List<Tile>
        {
            current
        };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }
        return totalPath;
    }

    private List<Tile> GetNeighbors(Tile tile)
    {
        var list = new List<Tile>();
        if (tile.X > 0)
        {
            list.Add(tiles[tile.X - 1, tile.Y]);
        }
        if (tile.X < columnCount - 1)
        {
            list.Add(tiles[tile.X + 1, tile.Y]);
        }
        if (tile.Y > 0)
        {
            list.Add(tiles[tile.X, tile.Y - 1]);
        }
        if (tile.Y < rowCount - 1)
        {
            list.Add(tiles[tile.X, tile.Y + 1]);
        }
        return list;
    }

}
