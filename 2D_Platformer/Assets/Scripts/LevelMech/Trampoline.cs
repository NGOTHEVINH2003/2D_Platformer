using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    protected Animator anim;
    [SerializeField] private float pushPower;
    [SerializeField] private float duration = .5f;
    [SerializeField] private bool fixTravelDuration = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        Debug.Log("Contact with Trampoline");
        if (player != null)
        {
            float speed = 1;
            if (fixTravelDuration) speed = pushPower / duration;

            player.GetPush(transform.up * pushPower * speed, duration);
            anim.SetTrigger("Activate");
        }
    }
}
