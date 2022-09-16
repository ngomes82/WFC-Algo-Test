using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapRandomizer
{
    public static void Randomize(Tilemap tilemap, Dictionary<string, Tile> allTiles, TileProbabilityDistribution tileProbabilities)
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


        tilemap.ClearAllTiles();


        var minBounds = new Vector3Int(0, 0, 0);
        var maxBounds = new Vector3Int(20, 20, 0);
        var randX = Random.Range(minBounds.x, maxBounds.x);
        var randY = Random.Range(minBounds.y, maxBounds.y);
        var startPos = new Vector3Int(randX, randY, 0);
        var closedList = new HashSet<Vector3Int>();
        var openList = new Queue<Vector3Int>();

        openList.Enqueue(startPos);

        while(openList.Count > 0)
        {
            var currentPos = openList.Dequeue();
            closedList.Add(currentPos);
            var consideredTiles = new Dictionary<string, float>();

            for (int i = 0; i < adjacencyDirections.Count; i++)
            {
                var dir = adjacencyDirections[i];
                var dirKey = adjacencyDirectionKeys[i];

                //Note: we are checking the opposite direction's probability.
                //Ex: Move down and check the "up" tile probabilies.
                var newPos = currentPos - dir;

                if (IsOutOfBounds(newPos, minBounds, maxBounds))
                    continue;

                var tile = tilemap.GetTile(newPos);

                if (tile == null)
                    continue;

                if (!tileProbabilities.ContainsKey(tile.name))
                    continue;

                if (!tileProbabilities[tile.name].ContainsKey(dirKey))
                    continue;

                var possibleTiles = tileProbabilities[tile.name][dirKey];
                var keysToRemove = new List<string>();
                foreach(var existingTile in consideredTiles)
                {
                    if (!possibleTiles.ContainsKey(existingTile.Key))
                        keysToRemove.Add(existingTile.Key);
                }

                foreach(var key in keysToRemove)
                {
                    consideredTiles.Remove(key);
                }

                foreach(var tileProb in tileProbabilities[tile.name][dirKey])
                {
                    if (consideredTiles.ContainsKey(tileProb.Key))
                        consideredTiles[tileProb.Key] = 0.0f;

                    consideredTiles[tileProb.Key] += tileProb.Value;
                }

                //If new tile is not in the closed list, add it to the queue
                if(!closedList.Contains(newPos))
                {
                    openList.Enqueue(newPos);
                }
            }

            //TODO:
            //If considered tiles is empty, use a random tile.
            //Otherwise, pick from the weighted dist
            //place tile
        }

    }

    private static bool IsOutOfBounds(Vector3Int pos, Vector3Int minBounds, Vector3Int maxBounds)
    {
        return pos.x < minBounds.x || pos.x > maxBounds.x || pos.y < minBounds.y || pos.y > maxBounds.y;
    }
}
