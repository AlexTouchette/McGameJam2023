using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    
    private Vector2 m_Movement;
    private bool m_IsMoving;
    void Update()
    {
        //  Input
        m_Movement.x = Input.GetAxisRaw("Horizontal");
        m_Movement.y = Input.GetAxisRaw("Vertical");
        
        animator.SetFloat("Horizontal", m_Movement.x);
        animator.SetFloat("Vertical", m_Movement.y);
        animator.SetFloat("Speed", m_Movement.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        // Movement
        rb.MovePosition(rb.position + m_Movement * moveSpeed * Time.fixedDeltaTime);
    }
}
