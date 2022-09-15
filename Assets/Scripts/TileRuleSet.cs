using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileRuleSet : Dictionary<string, AdjacentTileProbabilities> { }

public class AdjacentTileProbabilities
{
    public Dictionary<string, float> up;
    public Dictionary<string, float> down;
    public Dictionary<string, float> left;
    public Dictionary<string, float> right;
}



//example
// {
//   tile_001:
//   {
//      up:
//      {
//        "tile_002": 0.05,
//        "tile_003": 0.025
//      },
//      ....
//   },
//   ...
// }
