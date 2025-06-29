using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Animator animator;
    private bool isActive;



    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive) { return; }

        Player player = collision.GetComponent<Player>();

        if (collision != null && !isActive) ActivateCheckPoint();
    }


    private void ActivateCheckPoint()
    {
        isActive = true;
        animator.SetTrigger("Activate");
        GameManager.Instance.UpdateRespawnPoint(gameObject.transform);
    }
}
