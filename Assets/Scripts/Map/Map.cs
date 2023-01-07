    using System;
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    using UnityEngine.Tilemaps;

    public class Map : MonoBehaviour
{
    public BiomePreset[] biomes;
    public GameObject tilePrefab;
    public Transform parent;

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

    private float last;

    public Tilemap tilemap;
    public Tilemap borderTilemap;
    
    [SerializeField] private Tile DesertTile = null;
    [SerializeField] private Tile ForestTile = null;
    [SerializeField] private Tile GrasslandTile = null;
    [SerializeField] private Tile JungleTile = null;
    [SerializeField] private Tile MountainsTile = null;
    [SerializeField] private Tile OceanTile = null;
    [SerializeField] private Tile TundraTile = null;
    [SerializeField] private Tile BorderTile = null;

    private void Start()
    {
    }

    public void GenerateMap ()
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
                    case "Ocean":
                        tilemap.SetTile(tilePosition, OceanTile);
                        break;
                    case "Grassland":
                        tilemap.SetTile(tilePosition, GrasslandTile);
                        break;
                    case "Forest":
                        tilemap.SetTile(tilePosition, ForestTile);
                        break;
                    case "Jungle":
                        tilemap.SetTile(tilePosition, JungleTile);
                        break;
                    case "Tundra":
                        tilemap.SetTile(tilePosition, TundraTile);
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
}