using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private Tilemap LootTilemap;
    public Tile Loots;

    private TileManager m_Tm;
    private Vector3Int m_CurrentPosition;
    
    [Header("Dimensions")]
    public int width = 100;
    public int height = 100;
    public float scale = 1.0f;
    public Vector2 offset;

    [Header("Height Map")]
    public Wave[] heightWaves;
    public Gradient heightDebugColors;
    public float[,] heightMap;

    [Header("Moisture Map")]
    public Wave[] moistureWaves;
    public Gradient moistureDebugColors;
    private float[,] moistureMap;

    [Header("Heat Map")]
    public Wave[] heatWaves;
    public Gradient heatDebugColors;
    private float[,] heatMap;

    [SerializeField] private Tile DesertTile = null;
    [SerializeField] private Tile JungleTile = null;
    [SerializeField] private Tile MountainsTile = null;
    [SerializeField] private Tile BorderTile = null;
    
    private DeckManager m_Dm;
    
    public BiomePreset[] biomes;

    private float last;
    
    public Tilemap tilemap;
    public Tilemap borderTilemap;

    public int lootsAmount;

    private int m_HealthPoints;
    // Start is called before the first frame update
    void Start()
    {
        lootsAmount = 50;
        m_HealthPoints = 10;
        
        m_Tm = GameObject.Find("Grid").GetComponent<TileManager>();
        m_Dm = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        GenerateMap();
        
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
    
    public void GenerateMap()
    {
        // height map
        heightMap = NoiseGenerator.Generate(width, height, scale, heightWaves, offset);

        // moisture map
        moistureMap = NoiseGenerator.Generate(width, height, scale, moistureWaves, offset);

        // heat map
        heatMap = NoiseGenerator.Generate(width, height, scale, heatWaves, offset);
        
        Color[] pixels = new Color[width * height];

        for(int x = 0; x < width; ++x)
        {
            for(int y = 0; y < height; ++y)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                switch (GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y]).name)
                {
                    case "Desert":
                        tilemap.SetTile(tilePosition, DesertTile);
                        break;
                    case "Mountains":
                        tilemap.SetTile(tilePosition, MountainsTile);
                        break;
                    case "Jungle":
                        tilemap.SetTile(tilePosition, JungleTile);
                        break;
                    default:
                        break;
                }
            }
        }

        for (int x = 0 - 15; x < width + 15; x++)
        {
            for (int y = 0 - 15; y < height + 15; y++)
            {
                if (x < 0 || y < 0 || x >= width || y >= height)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    borderTilemap.SetTile(tilePosition, BorderTile);
                } 
            }
        }
    }
    
    
    BiomePreset GetBiome (float height, float moisture, float heat)
    {
        BiomePreset biomeToReturn = null;
        List<BiomeTempData> biomeTemp = new List<BiomeTempData>();

        foreach(BiomePreset biome in biomes)
        {
            if(biome.MatchCondition(height, moisture, heat))
            {
                biomeTemp.Add(new BiomeTempData(biome));
            }
        }

        float curVal = 0.0f;

        foreach(BiomeTempData biome in biomeTemp)
        {
            if(biomeToReturn == null)
            {
                biomeToReturn = biome.biome;
                curVal = biome.GetDiffValue(height, moisture, heat);
            }
            else
            {
                if(biome.GetDiffValue(height, moisture, heat) < curVal)
                {
                    biomeToReturn = biome.biome;
                    curVal = biome.GetDiffValue(height, moisture, heat);
                }
            }
        }

        if(biomeToReturn == null)
            biomeToReturn = biomes[0];
        return biomeToReturn;
    }
    
    public class BiomeTempData
    {
        public BiomePreset biome;

        public BiomeTempData (BiomePreset preset)
        {
            biome = preset;
        }
        
        public float GetDiffValue (float height, float moisture, float heat)
        {
            return (height - biome.minHeight) + (moisture - biome.minMoisture) + (heat - biome.minHeat);
        }
    }

    public void TakeDamage(int damage)
    {
        m_HealthPoints -= damage;
        if (m_HealthPoints <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        SceneManager.LoadScene("Level");
    }
}
