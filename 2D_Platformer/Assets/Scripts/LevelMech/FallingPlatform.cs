using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using Random = UnityEngine.Random;

public class FallingPlatform : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D[] colliders;

    [SerializeField] private float platformSpeed = .75f;
    [SerializeField] private float travelDistance;
    [SerializeField] Vector3[] wayPoints;
    private int wayPointIndex = 0;
    [SerializeField]private bool canMove = false;

    [Header("Platform fall details")]
    [SerializeField] private float delayFall = 1f;
    [SerializeField] private float impactSpeed = 3;
    [SerializeField] private float impactDuration = .1f;
    private float impactTimer;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<BoxCollider2D>();
    }

    private void Start()
    {
        SetupWayPoints();

        float randomDelay = Random.Range(0, .8f);
        Invoke("ActivatePlatform", randomDelay);
    }

    private void ActivatePlatform() => canMove = true;

    private void SetupWayPoints()
    {
        wayPoints = new Vector3[2];
        float yOffset = travelDistance / 2;

        wayPoints[0] = transform.position + new Vector3(0, yOffset, 0);
        wayPoints[1] = transform.position + new Vector3(0, -yOffset, 0);
    }

    private void Update()
    {
        HandleImpact(); 
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!canMove) return;

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[wayPointIndex], platformSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPoints[wayPointIndex]) < .1f)
        {
            wayPointIndex++;

            if (wayPointIndex >= wayPoints.Length)
            {
                wayPointIndex = 0;
            }
        }
    }
    private void HandleImpact()
    {
        if (impactTimer < 0) return;

        impactTimer -= Time.deltaTime;

        transform.position = Vector2.MoveTowards(transform.position, 
            transform.position + (Vector3.down * 10), 
            impactSpeed * Time.deltaTime);
    }
    private void SwitchOffPlatform()
    {
        anim.SetTrigger("Deactivate");
        rb.isKinematic = false;
        rb.gravityScale = 3f;
        rb.drag = .5f;

        foreach(BoxCollider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            Invoke("SwitchOffPlatform", delayFall);
            impactTimer = impactDuration;
        }
    }


}
