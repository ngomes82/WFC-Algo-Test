using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;

public class TileCountData
{
    public int total = 0;
    public Dictionary<string, int> perTileCounts = new Dictionary<string, int>();
}

//This maps:   tile => {   direction => { total, tile => count } }
public class AdjacentTileCounter : Dictionary<string, Dictionary<string, TileCountData>> { }

//This maps: tile => { direction => { tile => probability } }
public class TileProbabilityDistribution : Dictionary<string, Dictionary<string, Dictionary<string, float>>> { }

public class TilemapRuleAnalyzer : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile[] allTiles;
    

    // Start is called before the first frame update
    void Start()
    {
        var adjacencyDirections = new List<Vector3Int>()
        {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };

        var adjacencyDirectionKeys = new List<string>()
        {
            "up",
            "down",
            "left",
            "right"
        };


        var tileCounts = CountAdjacentTiles(tilemap, adjacencyDirections, adjacencyDirectionKeys);
        var tileDistribution = CalculateTileProbabilities( tileCounts );
        
    }

    public static AdjacentTileCounter CountAdjacentTiles(Tilemap tilemap, List<Vector3Int> adjacencyDirections, List<string> adjacencyDirectionKeys)
    {
        AdjacentTileCounter adjacentTileCounts = new AdjacentTileCounter();

        var curPos = new Vector3Int(0, 0, 0);
        for (int x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++)
        {
            for (int y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++)
            {
                curPos.x = x;
                curPos.y = y;

                var curTile = tilemap.GetTile(curPos);
                if (curTile == null)
                    continue;

                for (int i = 0; i < adjacencyDirections.Count; i++)
                {
                    var adjacencyVec = adjacencyDirections[i];
                    var nextPos = curPos + adjacencyVec;

                    if (!tilemap.cellBounds.Contains(nextPos))
                    {
                        continue;
                    }

                    //Sum all possible adjacencies
                    var adjacencyKey = adjacencyDirectionKeys[i];

                    if (!adjacentTileCounts.ContainsKey(curTile.name))
                        adjacentTileCounts[curTile.name] = new Dictionary<string, TileCountData>();

                    if (!adjacentTileCounts[curTile.name].ContainsKey(adjacencyKey))
                        adjacentTileCounts[curTile.name][adjacencyKey] = new TileCountData();

                    adjacentTileCounts[curTile.name][adjacencyKey].total += 1;

                    //Sum distinct adjacent tiles
                    var adjacentTile = tilemap.GetTile(nextPos);
                    if (adjacentTile == null)
                        continue;

                    if (!adjacentTileCounts[curTile.name][adjacencyKey].perTileCounts.ContainsKey(adjacentTile.name))
                        adjacentTileCounts[curTile.name][adjacencyKey].perTileCounts[adjacentTile.name] = 0;

                    adjacentTileCounts[curTile.name][adjacencyKey].perTileCounts[adjacentTile.name] += 1;
                }
            }
        }

        return adjacentTileCounts;
    }


    public static TileProbabilityDistribution CalculateTileProbabilities(AdjacentTileCounter adjacentTileCounts)
    {
        TileProbabilityDistribution adjacenctTileProbabilityDistribution = new TileProbabilityDistribution();

        foreach(var curTile in adjacentTileCounts)
        {
            foreach(var adjacentDir in adjacentTileCounts[curTile.Key])
            {
                float totalCount = adjacentTileCounts[curTile.Key][adjacentDir.Key].total;

                foreach(var adjTile in adjacentTileCounts[curTile.Key][adjacentDir.Key].perTileCounts)
                {
                    var tileCount = adjacentTileCounts[curTile.Key][adjacentDir.Key].perTileCounts[adjTile.Key];

                    var tileProbability = tileCount / totalCount;

                    if (!adjacenctTileProbabilityDistribution.ContainsKey(curTile.Key))
                        adjacenctTileProbabilityDistribution[curTile.Key] = new Dictionary<string, Dictionary<string, float>>();

                    if (!adjacenctTileProbabilityDistribution[curTile.Key].ContainsKey(adjacentDir.Key))
                        adjacenctTileProbabilityDistribution[curTile.Key][adjacentDir.Key] = new Dictionary<string, float>();

                    adjacenctTileProbabilityDistribution[curTile.Key][adjacentDir.Key][adjTile.Key] = tileProbability;
                }
            }
        }

        return adjacenctTileProbabilityDistribution;
    }
}
