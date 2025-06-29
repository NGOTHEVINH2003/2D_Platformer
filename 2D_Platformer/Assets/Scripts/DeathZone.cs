using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player playerMovement = collision.GetComponent<Player>();

        if (playerMovement != null)
        {
            playerMovement.PlayerDeath();
        }
    }
}
