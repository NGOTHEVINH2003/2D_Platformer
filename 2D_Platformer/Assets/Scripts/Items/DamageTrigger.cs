using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();


        if (player != null)
        {
            player.KnockBack(transform.position.x);
            Debug.Log("Player in the fucking trap!!!");
        }
    }
}
