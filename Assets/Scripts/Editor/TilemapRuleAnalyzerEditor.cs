#if UNITY_EDITOR
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(TilemapRuleAnalyzer))]
public class TilemapRuleAnalyzerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TilemapRuleAnalyzer analyzer = target as TilemapRuleAnalyzer;

        if (analyzer == null)
            return;

        if(GUILayout.Button("Analyze Tile Ruleset"))
        {
            var path = EditorUtility.SaveFilePanel("Save Tile Ruleset", Application.dataPath, "tileProbDist", "json");

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

            var counts = TilemapRuleAnalyzer.CountAdjacentTiles(analyzer.tilemap, adjacencyDirections, adjacencyDirectionKeys);
            var probabilities = TilemapRuleAnalyzer.CalculateTileProbabilities(counts);

            File.WriteAllText(path, JsonConvert.SerializeObject(probabilities));
        }

        if(GUILayout.Button("Randomize with Ruleset"))
        {
            var path = EditorUtility.OpenFilePanel("Load Tile Ruleset", Application.dataPath, "json");
            var fileContents = File.ReadAllText(path);
            var tileProbabilities = JsonConvert.DeserializeObject<TileProbabilityDistribution>(fileContents);

            Dictionary<string, Tile> allTiles = new Dictionary<string, Tile>();

            foreach(var tile in analyzer.allTiles)
            {
                allTiles[tile.name] = tile;
            }

            TilemapRandomizer.Randomize(analyzer.tilemap, allTiles, tileProbabilities);
        }

    }
}
#endif