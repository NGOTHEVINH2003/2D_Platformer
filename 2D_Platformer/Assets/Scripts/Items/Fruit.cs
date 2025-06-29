using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public enum FruitType {Apple, Kiwi, Cherry, Melon, Pineapple, Banana, Orange, Strawberry }


    [SerializeField] private FruitType type;
    [SerializeField] private bool randomVisual;
    [SerializeField] private GameObject PickupVFX;
    private GameManager gameManager;
    private Animator animator;
    

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        if (randomVisual || gameManager.IsFruitRandom())
        {
            SetRandomLookIndex();
        }
        else
        {
            SetDesignatedLook();
        }
    }

    private void SetRandomLookIndex()
    {
        int randomIndex = Random.Range(0, 8);

        animator.SetFloat("Fruit", randomIndex);
    }


    private void SetDesignatedLook() => animator.SetFloat("Fruit", (int)type +1 );

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player playerMovement = collision.GetComponent<Player>();
        if(playerMovement != null)
        {
            gameManager.AddFruit();
            Instantiate(PickupVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
