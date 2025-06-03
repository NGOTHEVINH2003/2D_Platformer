using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private PlayerMovement playerMovement;

    [SerializeField] private int fruitCollected;
    [SerializeField] private bool randomFruit;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public PlayerMovement GetPlayerMovement()
    {
        return playerMovement;
    }

    public bool IsFruitRandom() { return randomFruit; }

    public int GetFruit() { return fruitCollected; }
    public void AddFruit() => fruitCollected++;
}
