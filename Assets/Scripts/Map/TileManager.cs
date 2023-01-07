using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private Tilemap interactiveMap = null;
    [SerializeField] private Tile hoverTile = null;

    private Vector3Int previousMousePos = new Vector3Int();
    
    private GameObject m_Player;
    private Animator m_Animator;
    private bool m_IsMoving;
    
    void Start() {
        grid = gameObject.GetComponent<Grid>();
        m_Player = GameObject.Find("Player");
        m_Animator = m_Player.GetComponent<Animator>();
    }
    
    void Update() {
        // Mouse over -> highlight tile
        Vector3Int mousePos = GetMousePosition();

        if (!mousePos.Equals(previousMousePos)) {
            interactiveMap.SetTile(previousMousePos, null); // Remove old hoverTile
            interactiveMap.SetTile(mousePos, hoverTile);
            previousMousePos = mousePos;
        }
        
        // Left mouse click -> move to tile
        if (Input.GetMouseButton(0))
        {
            Vector3 destination = GetMousePosition() + new Vector3(0.5f, 0.875f, 0);
            if (!m_IsMoving)
                StartCoroutine(Move(m_Player.transform.position, destination));
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
        
        Debug.Log(deltaX + " " + deltaY);

        if (deltaX > deltaY)
        {
            m_Animator.SetFloat("Horizontal", direction.x);
            m_Animator.SetFloat("Speed", 1);
        } else
        {
            m_Animator.SetFloat("Vertical", direction.y);
            m_Animator.SetFloat("Speed", 1);
        }
        
        for (int i = 0; i < 100; i ++)
        {
            m_Player.transform.Translate(direction.x * 0.01f, direction.y * 0.01f, direction.z);
            yield return new WaitForSeconds(0.01f);
        }
        
        m_Animator.SetFloat("Horizontal", 0);
        m_Animator.SetFloat("Vertical", 0);
        m_Animator.SetFloat("Speed", 0);
        m_IsMoving = false;
    }
}
