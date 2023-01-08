using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private Map map;
    private Tilemap LootTilemap;
    public Tile Loots;

    private TileManager m_Tm;
    private Vector3Int m_CurrentPosition;

    private DeckManager m_Dm;

    public int lootsAmount;
    // Start is called before the first frame update
    void Start()
    {
        lootsAmount = 50;
        map = GameObject.Find("Map").GetComponent<Map>();
        map.GenerateMap();

        m_Tm = GameObject.Find("Grid").GetComponent<TileManager>();
        m_Dm = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        
        LootTilemap = GameObject.Find("Loots").GetComponent<Tilemap>();
        GenerateLoots();
    }

    private void Update()
    {
        m_CurrentPosition = Vector3Int.FloorToInt(m_Tm.CurrentPosition);

        if (LootTilemap.GetTile(m_CurrentPosition) != null)
        {
            m_Dm.ShowShop();
        }
    }

    void GenerateLoots()
    {
        bool isAssigned = false;
        for (int i = 0; i < lootsAmount; i++)
        {
            isAssigned = false;
            while (!isAssigned)
            {
                int x = Random.Range(0, 99);
                int y = Random.Range(0, 99);
                Vector3Int position = new Vector3Int(x, y, 0);
                if (LootTilemap.GetTile(position) == null)
                {
                    LootTilemap.SetTile(position, Loots);
                    isAssigned = true;
                }
            }
        }
    }
}
