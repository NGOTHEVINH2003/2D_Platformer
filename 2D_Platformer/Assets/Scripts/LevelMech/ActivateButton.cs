using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActivateButton : MonoBehaviour
{
    public event EventHandler button_press;
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            Debug.Log("Trigger Button");
            anim.SetTrigger("Activate");
            button_press?.Invoke(this, EventArgs.Empty);
        }
    }
}
