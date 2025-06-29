using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTrap : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float speedMultiplier = 1;
    [SerializeField] private float coolDown = .25f;
    [SerializeField] private Transform[] waysPoints;
    [SerializeField] private Vector3[] wayPointsPosition;

    private int wayPointIndex = 1;
    private int moveDir = 1;
    private bool canMove = true;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        UpdateWayPointPosition();
        transform.position = wayPointsPosition[0];
    }


    private void Update()
    {
        anim.SetBool("IsActive", canMove);

        if(canMove) transform.position = Vector2.MoveTowards(transform.position, wayPointsPosition[wayPointIndex], moveSpeed * speedMultiplier * Time.deltaTime);


        if (Vector2.Distance(transform.position, wayPointsPosition[wayPointIndex]) < .1f)
        {

            if (wayPointIndex == wayPointsPosition.Length - 1 || wayPointIndex == 0)
            {
                moveDir = moveDir * -1;
                StartCoroutine(StopMovement(coolDown));
            }
            wayPointIndex = wayPointIndex + moveDir;



        }

    }

    private void UpdateWayPointPosition()
    {
        List<SawWayPoint> listWayPoints = new List<SawWayPoint>(GetComponentsInChildren<SawWayPoint>()); 

        if(listWayPoints.Count != waysPoints.Length)
        {
            waysPoints = new Transform[listWayPoints.Count];

            for(int i = 0; i< listWayPoints.Count; i++)
            {
                waysPoints[i] = listWayPoints[i].transform;
            }

        }

        wayPointsPosition = new Vector3[waysPoints.Length];

        for(int i =0;i < wayPointsPosition.Length; i++)
        {
            wayPointsPosition[i] = waysPoints[i].position;
        }
    }

    private IEnumerator StopMovement(float delay)
    {
        canMove = false;

        yield return new WaitForSeconds(delay);

        canMove = true;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}


