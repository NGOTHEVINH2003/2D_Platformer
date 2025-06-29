using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TrapArrow : Trampoline
{
    [Header("Additional Info")]
    [SerializeField] private bool rotateClockWise;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float rotateSpeed = 120f;
    [SerializeField] private float coolDown = 3f;
    private int rotateDir = 1;

    [SerializeField] private float growSpeed = 10f;
    [SerializeField] private Vector3 targetScale;

    private void Start()
    {
        transform.localScale = new Vector3(.3f,.3f,.3f);
    }
    private void OnEnable()
    {
        transform.localScale = new Vector3(.3f, .3f, .3f);
    }

    private void Update()
    {
        if (transform.localScale.x < targetScale.x) transform.localScale = Vector3.Lerp(transform.localScale, targetScale, growSpeed * Time.deltaTime);
        rotateDir = rotateClockWise ? -1 : 1;
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime * rotateDir);
    }
     
    private void DestroyObject()
    {
        GameManager.Instance.RespawnObject(gameObject, transform, coolDown, true);
        //gameObject.SetActive(false);
    }

   
}
