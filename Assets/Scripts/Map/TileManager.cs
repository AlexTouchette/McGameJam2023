using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileManager : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private Tilemap interactiveMap = null;
    [SerializeField] private Tile hoverTile = null;

    private Vector3Int previousMousePos = new Vector3Int();

    private Vector3Int m_HighlightedTile;
    public Vector3 CurrentPosition;
    private GameObject m_Player;
    private Animator m_Animator;
    private bool m_IsMoving;
    DeckManager deckManager;

    private GameManager m_Gm;
    private AudioManager m_Am;
    public GameObject WinScreen;

    void Start() {
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        grid = gameObject.GetComponent<Grid>();
        m_Player = GameObject.Find("Player");
        m_Animator = m_Player.GetComponent<Animator>();
        m_Gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_Am = GameObject.Find("Audio").GetComponent<AudioManager>();
    }
    
    void Update() {
        // Mouse over -> highlight tile
        Vector3Int mousePos = GetMousePosition();
        if (!mousePos.Equals(previousMousePos)) {
            interactiveMap.ClearAllTiles();

            Vector3 difference = mousePos - m_Player.transform.position;
            if (!(difference.x == 0 && difference.y == 0))
            {
                if (difference.x > 1.1)
                {
                    difference.x = 1;
                } else if (difference.x < -1.1)
                {
                    difference.x = -1;
                }

                if (difference.y > 1.1)
                {
                    difference.y = 1;
                } else if (difference.y < -1.1)
                {
                    difference.y = -1;
                }
            }

            m_HighlightedTile = Vector3Int.FloorToInt(m_Player.transform.position + difference);
            interactiveMap.SetTile(m_HighlightedTile, hoverTile);
            previousMousePos = mousePos;
        }
        
        // Left mouse click -> move to tile
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            string tileString = m_Gm.tilemap.GetTile(m_HighlightedTile).ToString().Split(" ")[0];
            if (!m_IsMoving)
            {
                //Debug.Log(tileString);
                // TODO: premi�re partie du OR � enlever quand il n'y aura que 3 bi�mes
                if (!deckManager.movementPoints.ContainsKey(tileString))
                {
                    Win();
                    return;
                }
                    
                if(deckManager.movementPoints[tileString] > 0)
                {
                    deckManager.movementPoints[tileString]--;
                    deckManager.UpdateUIMovementPoints();
                    CurrentPosition = m_HighlightedTile + new Vector3(0.5f, 0.875f, 0);
                    StartCoroutine(Move(m_Player.transform.position, CurrentPosition));
                    m_Am.CheckWaterDistance();
                } 
                else if(deckManager.itemState.UseCar())
                {
                    CurrentPosition = m_HighlightedTile + new Vector3(0.5f, 0.875f, 0);
                    StartCoroutine(Move(m_Player.transform.position, CurrentPosition));
                }
            }
        }
    }
    
    Vector3Int GetMousePosition () {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return grid.WorldToCell(mouseWorldPos);
    }
    
    IEnumerator Move(Vector3 currentPosition, Vector3 destination)
    {
        m_IsMoving = true;
        Vector3 direction = destination - currentPosition;
        float deltaX = Math.Abs(direction.x);
        float deltaY = Math.Abs(direction.y);

        if (deltaX > deltaY)
        {
            m_Animator.SetFloat("Horizontal", direction.x);
            m_Animator.SetFloat("Speed", 1);
        } else
        {
            m_Animator.SetFloat("Vertical", direction.y);
            m_Animator.SetFloat("Speed", 1);
        }
        
        for (int i = 0; i < 50; i ++)
        {
            m_Player.transform.Translate(direction.x * 0.02f, direction.y * 0.02f, direction.z);
            yield return new WaitForSeconds(0.008f);
        }
        
        m_Animator.SetFloat("Horizontal", 0);
        m_Animator.SetFloat("Vertical", 0);
        m_Animator.SetFloat("Speed", 0);
        m_IsMoving = false;
    }
    
    public void CheckWin()
    {
        if (CurrentPosition.x <= 1 || CurrentPosition.x >= 99 || CurrentPosition.y <= 1 ||
            CurrentPosition.y >= 99)
        {
            Win();
        }
    }

    public void Win()
    {
        WinScreen.SetActive(true);
    }
}
