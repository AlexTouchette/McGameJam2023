using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private Map map;
    private Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {
        map = GameObject.Find("Map").GetComponent<Map>();
        map.GenerateMap();
        
        tilemap = map.tilemap;
    }


}
