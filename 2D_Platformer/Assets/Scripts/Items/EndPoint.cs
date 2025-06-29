using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private Animator animator;
    private bool isActive = false;


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
    }
}
